using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPhysics {

    public struct Pair {
        public Rigidbody a;
        public Rigidbody b;
        public Pair(Rigidbody a, Rigidbody b) {
            this.a = a;
            this.b = b;
        }
    }
    public class BroadPairsGenerator {
        List<Pair> pairs = new List<Pair>();
        public List<Pair> GeneratePairs(List<Rigidbody> bodies) {
            pairs.Clear();
            AABB a_aabb;
            AABB b_aabb;
            for (int i = 0; i < bodies.Count; i++) {
                for (int j = 0; j < bodies.Count; j++) {
                    Rigidbody a = bodies[i];
                    Rigidbody b = bodies[j];
                    if (a == b) continue;
                    if (!Layer.Intersect(a.layer, b.layer)) { Debug.Log("x"); continue; }
                    a_aabb = a.GetAABB();
                    b_aabb = b.GetAABB();
                    if (NarrowDetection.AABBvsAABB(a_aabb, b_aabb)) {
                        pairs.Add(new Pair(a, b));
                    }
                }
            }
            return CullPairs(pairs);
        }

        private List<Pair> CullPairs(List<Pair> pairs) {
            pairs = pairs.OrderBy(i => i.a.GetType()).ToList();
            int i = 0;
            List<Pair> uniquePairse = new List<Pair>();
            while (i < pairs.Count) {
                Pair uniquePair = pairs[i];
                uniquePairse.Add(uniquePair);
                ++i;
                while (i < pairs.Count) {
                    Pair potentialDuplcate = pairs[i];
                    if (uniquePair.a != potentialDuplcate.b || uniquePair.b != potentialDuplcate.a) break;
                    ++i;
                }
            }
            return uniquePairse;
        }
    }
}