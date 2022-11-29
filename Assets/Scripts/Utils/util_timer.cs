using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class util_timer {

    #region PRIVATE
        public static Dictionary<string, util_timer> timers = new Dictionary<string, util_timer>();
        private static int ID = 0;

        #region TIMER
            private string _id;
            private float _nextTick;
            private float _delay;
            private int _iterations;
            private Action _func;

            private bool _infinite;
        #endregion
    #endregion

    public static void fixedUpdate() {
        if(timers == null || timers.Count <= 0) return;
        foreach (util_timer timer in timers.Values.ToList()) {
            if (timer != null) timer.tick();
            else timers.Remove(timer._id);
        }
    }

    public static util_timer simple(float delay, Action func) {
        return create(1, delay, func);
    }

    public static util_timer create(int reps, float delay, Action func) {
        util_timer t = new util_timer {
            _iterations = reps,
            _delay = delay,
            _func = func,
            _id = (ID++).ToString(),
            _infinite = reps < 0
        };

        t.start();
        return t;
    }

    public static void clear() {
        foreach (util_timer timer in timers.Values.ToList())
            if (timer != null) timer.stop();

        timers.Clear();
        ID = 0;
    }

    public void tick() {
        float currTime = Time.time;
        if (currTime < this._nextTick) return;

        if (this._func != null) this._func.Invoke();
        if (!this._infinite) this._iterations--;

        if (this._iterations == 0) this.stop();
        else this._nextTick = currTime + this._delay;
    }

    public void stop() {
        if(!timers.ContainsKey(this._id)) return;
        timers.Remove(this._id);
    }

    public void start() {
        if(timers.ContainsKey(this._id)) throw new Exception("Timer already started");
        this._nextTick = Time.time + this._delay;

        timers.Add(this._id, this);
    }

    #if DEVELOPMENT_BUILD || UNITY_EDITOR
        public static string debug() {
            string data = "\n--------------- ACTIVE TIMERS: " + timers.Count;
            data += "\nCURRENT ID: " + ID;
            foreach (util_timer timer in timers.Values.ToList()) data += "\n [" + timer._id +"] DELAY: "+ timer._delay + " | ITERATIONS: " + timer._iterations + " | TIME: " + (timer._nextTick - Time.time) + "s";

            return data;
        }
    #endif
}