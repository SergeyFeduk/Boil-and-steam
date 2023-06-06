using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
namespace DPhysics {
    public class Rigidbody {
        public Transform transform;
        public Material material;
        public MassData massData;
        public Shape shape;

        public Vector2 velocity;
        public Vector2 netForce;
        public Layer layer;
        public Rigidbody() {
            transform = new Transform();
        }
        public AABB GetAABB() {
            AABB aabb = new AABB(shape.ComputeAABB());
            aabb.max += transform.position;
            aabb.min += transform.position;
            return aabb;
        }
        public void AddForce(Vector2 force) {
            netForce += force;
        }
    }

    public class Transform {
        public Vector2 position;
        public float rotation;
        public Vector2 size;
    }

    public class Layer {
        public int mask;
        int layerId;
        public static int maxLayerId = 0;
        public Layer() {
            layerId = maxLayerId;
            maxLayerId++;
        }
        public static bool Intersect(Layer layerA, Layer layerB) {
            if (layerA == null || layerB == null) return true;
            return (layerB.layerId & (1 << layerA.mask - 0)) == 0 && (layerA.layerId & (1 << layerB.mask - 0)) == 0;
        }

        public void SetBit(int bit, bool value) {
            if (value) {
                mask |= (1 << bit);
                return;
            }
            mask &= ~(1 << bit);
        }

        public void SetLayer(Layer layer, bool value) {
            SetBit(layer.layerId, value);
            layer.SetBit(layerId, value);
        }
    }

    public struct MassData {
        public float mass;
        public float inverseMass;

        public float inertia;
        public float inverseInertia;

        public void ComputeInverse() {
            if (mass == 0) { inverseMass = 0; } else { inverseMass = 1 / mass; }
            if (inverseInertia == 0) { inverseInertia = 0; } else { inverseInertia = 1 / inertia; }
        }
    }

    public struct Material {
        public float density;
        public float restitution;
    }
}