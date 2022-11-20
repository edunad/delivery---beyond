using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class util_timer {

    #region PRIVATE
        private static Dictionary<string, util_timer> _timers = new Dictionary<string, util_timer>();
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

    public static void update() {
        if(_timers == null || _timers.Count <= 0) return;

        try {
            foreach (util_timer timer in _timers.Values.ToList()) {
                if (timer != null) timer.tick();
                else _timers.Remove(timer._id);
            }
        } catch (Exception err) {
            Debug.LogError(err);
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
        foreach (util_timer timer in _timers.Values.ToList())
            if (timer != null) timer.stop();

        _timers.Clear();
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
        if(!_timers.ContainsKey(this._id)) return;
        _timers.Remove(this._id);
    }

    public void start() {
        if(_timers.ContainsKey(this._id)) throw new Exception("Timer already started");
        this._nextTick = Time.time + this._delay;

        _timers.Add(this._id, this);
    }
}