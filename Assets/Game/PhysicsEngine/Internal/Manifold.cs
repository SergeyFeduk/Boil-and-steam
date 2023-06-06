using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DPhysics {
    public class Manifold {
        public Rigidbody a;
        public Rigidbody b;
        public float penetration;
        public Vector2 normal;
        public void Swap() {
            Rigidbody c = a;
            a = b;
            b = c;
        }
    }
    public static class ManifoldMaker {
        public static bool CirclevsCircle(Manifold manifold) {
            Vector2 normal = manifold.b.transform.position - manifold.a.transform.position;
            Circle circleA = (Circle)manifold.a.shape;
            Circle circleB = (Circle)manifold.b.shape;
            float r = circleA.radius + circleB.radius;
            r *= r;
            if (normal.sqrMagnitude > r) return false;
            float d = normal.magnitude;
            if (d != 0) {
                manifold.penetration = r - d;
                manifold.normal = normal / d;
                return true;
            } else {
                manifold.penetration = circleA.radius;
                manifold.normal = new Vector2(1, 0);
                return true;
            }
        }

        public static bool AABBvsAABB(Manifold manifold) {
            Vector2 direction = manifold.b.transform.position - manifold.a.transform.position;
            AABB abox = (AABB)manifold.a.shape;
            AABB bbox = (AABB)manifold.b.shape;

            float aExtentX = (abox.max.x - abox.min.x) / 2;
            float bExtentX = (bbox.max.x - bbox.min.x) / 2;
            float xOverlap = aExtentX + bExtentX - Mathf.Abs(direction.x);
            if (xOverlap > 0) {
                float aExtentY = (abox.max.y - abox.min.y) / 2;
                float bExtentY = (bbox.max.y - bbox.min.y) / 2;
                float yOverlap = aExtentY + bExtentY - Mathf.Abs(direction.y);
                if (yOverlap > 0) {
                    if (xOverlap > yOverlap) {
                        if (direction.x < 0) {
                            manifold.normal = new Vector2(-1, 0);
                        } else {
                            manifold.normal = new Vector2(0, 0);
                        }
                        manifold.penetration = xOverlap;
                        return true;
                    } else {
                        if (direction.y < 0) {
                            manifold.normal = new Vector2(0, -1);
                        } else {
                            manifold.normal = new Vector2(0, 1);
                        }
                        manifold.penetration = yOverlap;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool AABBvsCircle(Manifold manifold) {
            if (manifold.a.shape.GetType() == typeof(Circle)) manifold.Swap();
            Vector2 direction = manifold.b.transform.position - manifold.a.transform.position;
            Vector2 closest = direction;
            AABB box = (AABB)manifold.a.shape;
            Circle circle = (Circle)manifold.b.shape;
            float xExtent = (box.max.x - box.min.x) / 2;
            float yExtent = (box.max.y - box.min.y) / 2;
            closest.x = Mathf.Clamp(-xExtent, xExtent, closest.x);
            closest.y = Mathf.Clamp(-yExtent, yExtent, closest.y);
            bool inside = false;
            if (direction == closest) {
                inside = true;
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
                    if (closest.x > 0) {
                        closest.x = xExtent;
                    } else {
                        closest.x = -xExtent;
                    }
                } else {
                    if (closest.y > 0) {
                        closest.y = yExtent;
                    } else {
                        closest.y = -yExtent;
                    }
                }
            }
            Vector2 normal = direction - closest;
            float d = normal.sqrMagnitude;

            float radius = circle.radius;
            if (d > radius * radius && !inside) return false;
            d = Mathf.Sqrt(d);
            
            if (inside) {
                manifold.normal = -direction;
                manifold.penetration = radius - d;
            } else {
                manifold.normal = direction;
                manifold.penetration = radius - d;
            }
            return true;
        }
    }
    
}