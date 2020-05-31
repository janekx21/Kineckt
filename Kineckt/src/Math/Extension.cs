using Microsoft.Xna.Framework;

namespace Kineckt.Math {
    public static class Extension {
        public static Vector3 MoveTowards(this Vector3 value, Vector3 target, float maxDelta) {
            var dist = Vector3.Distance(value, target);
            maxDelta = System.Math.Min(maxDelta, dist);
            var diff = target - value;
            if (!(diff.LengthSquared() > 0)) return value; // i am at my target
            diff.Normalize();
            return diff * maxDelta + value;
        }

        public static Vector2 MoveTowards(this Vector2 value, Vector2 target, float maxDelta) {
            var dist = Vector2.Distance(value, target);
            maxDelta = System.Math.Min(maxDelta, dist);
            var diff = target - value;
            if (!(diff.LengthSquared() > 0)) return value; // i am at my target
            diff.Normalize();
            return diff * maxDelta + value;
        }
    }
}