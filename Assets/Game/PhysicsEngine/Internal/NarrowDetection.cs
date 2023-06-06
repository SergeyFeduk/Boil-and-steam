using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DPhysics {
    public static class NarrowDetection {
        public static bool AABBvsAABB(AABB a, AABB b) {
            if (a.max.x < b.min.x || a.min.x > b.max.x) return false;
            if (a.max.y < b.min.y || a.min.y > b.max.y) return false;
            return true;
        }
        public static bool CirclevsCircle(Circle a, Circle b) {
            float r = a.radius + b.radius;
            r *= r;
            float xs = a.position.x + b.position.x;
            float ys = a.position.y + b.position.y;
            return r < xs * xs + ys * ys;
        }
    }
}