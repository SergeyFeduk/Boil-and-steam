using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DPhysics {

    public class Shape {
        public delegate bool ManifoldJumpDelegate(Manifold manifold);
        public virtual AABB ComputeAABB() { return null; }
        public virtual ManifoldJumpDelegate Jump(System.Type type) { return null; }
    }

    public class AABB : Shape {
        public Vector2 min = Vector2.zero;
        public Vector2 max = Vector2.zero;
        public AABB() { }
        public AABB(AABB old) {
            min = old.min;
            max = old.max;
        }
        public Vector2 GetSize() {
            return max - min;
        }
        public override AABB ComputeAABB() {
            return this;
        }
        public Dictionary<System.Type, ManifoldJumpDelegate> jumpTable = new Dictionary<System.Type, ManifoldJumpDelegate>() {
            { typeof(AABB),     ManifoldMaker.AABBvsAABB},
            { typeof(Circle),   ManifoldMaker.AABBvsCircle}
        };
        public override ManifoldJumpDelegate Jump(Type type) {
            return jumpTable[type];
        }
        
    }

    public class Circle : Shape {
        public Vector2 position;
        public float radius;
        private AABB cachedAABB = new AABB();
        public override AABB ComputeAABB() {
            Vector2 radiusVector = new Vector2(radius, radius);
            cachedAABB.min = position - radiusVector;
            cachedAABB.max = position + radiusVector;
            return cachedAABB;
        }
        public Dictionary<System.Type, ManifoldJumpDelegate> jumpTable = new Dictionary<System.Type, ManifoldJumpDelegate>() {
            { typeof(AABB),     ManifoldMaker.AABBvsCircle},
            { typeof(Circle),   ManifoldMaker.CirclevsCircle}
        };
        public override ManifoldJumpDelegate Jump(Type type) {
            return jumpTable[type];
        }
    }
}

