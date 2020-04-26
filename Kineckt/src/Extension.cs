using System;
using Microsoft.Xna.Framework;

namespace Kineckt {
    public static class Extension {
        public static Vector3 moveTowards(this Vector3 value, Vector3 target, float maxDelta) {
            var dist = Vector3.Distance(value, target);
            maxDelta = Math.Min(maxDelta, dist);
            var diff = target - value;
            if (!(diff.LengthSquared() > 0)) return value; // i am at my target
            diff.Normalize();
            return diff * maxDelta + value;
        }
    }
}