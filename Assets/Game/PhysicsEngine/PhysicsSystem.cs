using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DPhysics {
    public class PhysicsSystem : MonoBehaviour {
        private Rigidbody a = new Rigidbody();
        private Rigidbody b = new Rigidbody();
        private Rigidbody c = new Rigidbody();
        private BroadPairsGenerator bpg = new BroadPairsGenerator();
        private void Start() {
            Layer layerA = new Layer();
            Layer layerB = new Layer();
            layerA.SetLayer(layerB, true);
            //Debug.Log(layerA.mask);
            //Debug.Log(layerB.mask);
            //Debug.Log(Layer.Intersect(layerA, layerB));
            //Debug.Log(Layer.Intersect(layerB, layerA));
            a.shape = new AABB() {  max = new Vector2(0.5f, 0.5f), min = new Vector2(-0.5f, -0.5f) };
            a.massData.mass = 0;
            a.massData.inertia = 1;
            a.massData.ComputeInverse();
            a.material.restitution = 1;
            a.velocity = new Vector2(0, 0);
            a.transform.position = new Vector2(2, 0);
            a.layer = layerA;

            ((AABB)a.shape).min += a.transform.position;
            ((AABB)a.shape).max += a.transform.position;
            //Debug.Log(((AABB)a.shape).min + " " + ((AABB)a.shape).max);
            b.shape = new Circle() {  position = new Vector2(0, 0), radius = 2 };
            b.massData.mass = 3;
            b.massData.inertia = 1;
            b.massData.ComputeInverse();
            b.material.restitution = 1;
            b.velocity = new Vector2(0.0f,0);
            b.transform.position = new Vector2(0, 0);
            b.layer = layerA;

            c.shape = new Circle() { position = new Vector2(4, 0), radius = 1 };
            c.massData.mass = 1;
            c.massData.inertia = 1;
            c.massData.ComputeInverse();
            c.material.restitution = 1;
            c.velocity = new Vector2(0.0f, 0);
            c.transform.position = new Vector2(4, 0);
            c.layer = layerA;
        }
        private void Update() {
            //Debug.Log(a.GetAABB().min + " " +  b.GetAABB().min);
            //Debug.Log(NarrowDetection.AABBvsAABB(a.GetAABB(),b.GetAABB()));
            c.AddForce(new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical")) * 20);
            /*if (Input.GetMouseButton(0)) {
                c.AddForce((ScreenUtils.WorldMouse() - c.transform.position).normalized * 10);
            }*/
            List<Pair> pairs = bpg.GeneratePairs(new List<Rigidbody>() { a, b, c });
            for (int i = 0; i < pairs.Count; i++) {
                //Debug.Log(pairs[i].a.shape.GetType() == pairs[i].b.shape.GetType());
                Debug.Log(pairs[i].a.shape.GetType() + " " + pairs[i].b.shape.GetType());
                Manifold manifold = new Manifold();
                manifold.a = pairs[i].a;
                manifold.b = pairs[i].b;
                bool collided = false;
                Shape.ManifoldJumpDelegate manifoldDelegate = manifold.a.shape.Jump((manifold.b.shape.GetType()));
                if (manifoldDelegate == null) {
                    throw new System.Exception("No solver implemented for collision case: " + manifold.a.shape.GetType() + " + " + manifold.b.shape.GetType());
                }
                collided = manifoldDelegate.Invoke(manifold);
                if (collided) ResolveCollision(manifold);
            }

            a.velocity += a.netForce * Time.deltaTime;
            b.velocity += b.netForce * Time.deltaTime;
            c.velocity += c.netForce * Time.deltaTime;
            a.netForce = Vector2.zero;
            b.netForce = Vector2.zero;
            c.netForce = Vector2.zero;
            a.transform.position += a.velocity * Time.deltaTime;
            ((AABB)a.shape).min += a.velocity * Time.deltaTime;
            ((AABB)a.shape).max += a.velocity * Time.deltaTime;

            b.transform.position += b.velocity * Time.deltaTime;
            ((Circle)b.shape).position = b.transform.position;

            c.transform.position += c.velocity * Time.deltaTime;
            ((Circle)c.shape).position = c.transform.position;
        }

        private void ResolveCollision(Manifold manifold) {
            Vector2 relativeVelocity = manifold.b.velocity - manifold.a.velocity;

            float velocityAlongNormal = Vector2.Dot(relativeVelocity, manifold.normal);
            if (velocityAlongNormal > 0) return;

            float restitution = Mathf.Min(manifold.a.material.restitution, manifold.b.material.restitution);
            float scalarImpulse = -(1 + restitution) * velocityAlongNormal;
            scalarImpulse /= manifold.a.massData.inverseMass + manifold.b.massData.inverseMass;

            Vector2 impulse = scalarImpulse * manifold.normal;
            manifold.a.velocity -= manifold.a.massData.inverseMass * impulse;
            manifold.b.velocity += manifold.b.massData.inverseMass * impulse;
            PositionalCorrection(manifold);
        }
        private void PositionalCorrection(Manifold manifold) {
            const float percent = 0.02f;
            const float slop = 0.01f;
            Vector2 correction = Mathf.Max(manifold.penetration - slop, 0.0f) / (manifold.a.massData.inverseMass + manifold.b.massData.inverseMass) * percent * manifold.normal;
            manifold.a.transform.position -= manifold.a.massData.inverseMass * correction;
            manifold.b.transform.position += manifold.b.massData.inverseMass * correction;
        }
        private void OnDrawGizmos() {
            if (a.shape == null || b.shape == null) return;
            //Gizmos.DrawCube(a.transform.position + ((AABB)a.shape).GetSize(), ((AABB)a.shape).GetSize());
            Gizmos.DrawLine(c.transform.position, ScreenUtils.WorldMouse());
            AABB aabb = a.GetAABB();
            Vector2 position = (aabb.max + aabb.min) / 2;
            Gizmos.DrawCube(a.transform.position, aabb.GetSize());
             aabb = b.GetAABB();
             position = (aabb.max + aabb.min) / 2;
            //Gizmos.DrawCube(position, aabb.GetSize());
            Gizmos.DrawSphere(b.transform.position, ((Circle)b.shape).radius / 1.5f);
             aabb = c.GetAABB();
             position = (aabb.max + aabb.min) / 2;
            
            //Gizmos.DrawCube(position, aabb.GetSize());
            Gizmos.DrawSphere(c.transform.position, ((Circle)c.shape).radius / 1.5f);
            //Gizmos.DrawCube(a.transform.position, ((AABB)a.shape.ComputeAABB()).GetSize());
            //Gizmos.DrawCube(b.transform.position, ((AABB)b.shape.ComputeAABB()).GetSize());
            //Gizmos.DrawCube(c.transform.position, ((AABB)c.shape.ComputeAABB()).GetSize());
            //Gizmos.DrawCube(a.transform.position - ((AABB)a.shape.ComputeAABB()).GetSize() / 2, ((AABB)a.shape.ComputeAABB()).GetSize());
            //Gizmos.DrawCube(b.transform.position - ((AABB)a.shape.ComputeAABB()).GetSize() / 2, ((AABB)b.shape.ComputeAABB()).GetSize());
            //Gizmos.DrawCube(c.transform.position - ((AABB)a.shape.ComputeAABB()).GetSize() / 2, ((AABB)c.shape.ComputeAABB()).GetSize());
        }
    }
}