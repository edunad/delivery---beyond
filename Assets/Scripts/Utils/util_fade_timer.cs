using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class util_fade_timer {

    #region PRIVATE
        public static Dictionary<string, util_fade_timer> timers = new Dictionary<string, util_fade_timer>();
        private static int ID = 0;

        #region TIMER
            private string _id;
            private float _speed;
            private float _initial;
            private float _targetValue;
            private float _timer;
            private Action<float> _onTick;
            private Action<float> _onComplete;
        #endregion
    #endregion

    public static void fixedUpdate() {
        if(timers == null || timers.Count <= 0) return;

        foreach (util_fade_timer timer in timers.Values.ToList()) {
            if (timer != null) timer.tick();
            else timers.Remove(timer._id);
        }
    }

    public static util_fade_timer fade(float speed, float initial, float target, Action<float> onTick, Action<float> onComplete = null) {
        util_fade_timer t = new util_fade_timer {
            _speed = speed,
            _initial = initial,
            _targetValue = target,
            _timer = 0f,
            _onTick = onTick,
            _onComplete = onComplete,
            _id = (ID++).ToString()
        };

        t.start();
        return t;
    }

    public static void clear() {
        foreach (util_fade_timer timer in timers.Values.ToList())
            if (timer != null) timer.stop();

        timers.Clear();
        ID = 0;
    }

    public void tick() {
        this._timer += this._speed * Time.deltaTime;

        if(this._onTick != null) this._onTick.Invoke(Mathf.Lerp(this._initial, this._targetValue, this._timer));
        if(this._timer >= 1f) {
            this.stop();
            if(this._onComplete != null) this._onComplete.Invoke(this._targetValue);
        }
    }

    public void stop() {
        if(!timers.ContainsKey(this._id)) return;
        timers.Remove(this._id);
    }

    public void start() {
        if(timers.ContainsKey(this._id)) throw new Exception("Fade already started");
        timers.Add(this._id, this);
    }

    #if DEVELOPMENT_BUILD || UNITY_EDITOR
        public static string debug() {
            string data = "\n--------------- ACTIVE FADE TIMERS: " + timers.Count;
            data += "\nCURRENT ID: " + ID;
            foreach (util_fade_timer timer in timers.Values.ToList()) data += "\n [" + timer._id +"] VALUE: "+ timer._targetValue + " | SPEED: " + timer._speed;

            return data;
        }
    #endif
}