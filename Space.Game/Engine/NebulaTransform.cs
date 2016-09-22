using System;
using GameMath;
using Common;

namespace Nebula.Engine {
    public class NebulaTransform  : NebulaBehaviour {

        public Vector3 position { get; private set; }
        public Vector3 rotation { get; private set; }
        

        public override void Start() {
            //position = new Vector3(0f, 0f, 0f);
            //rotation = new Vector3(0f, 0f, 0f);
        }

        public void SetPosition(Vector3 pos) {
            position = new Vector3(pos);
        }

        public void SetPosition(Vector pos) {
            position = new Vector3(pos.X, pos.Y, pos.Z);
        }

        public void SetPosition(float[] pos) {
            if(pos == null || pos.Length != 3) {
                pos = new float[] { 0, 0, 0 };
            }
            position = new Vector3(pos);
        }

        public void SetRotation(Vector3 rot) {
            rotation = new Vector3(rot);
        }

        public void SetRotation(Quat rot) {
            rotation = rot.eulerAngles;
        }

        public void SetRotation(float[] rot) {
            if(rot == null || rot.Length != 3) {
                rot = new float[] { 0, 0, 0 };
            }
            rotation = new Vector3(rot[0], rot[1], rot[2]);
        }

        public float DistanceTo(NebulaTransform other) {
            return Vector3.Distance(position, other.position);
        }

        public Vector3 DirectionTo(NebulaTransform other) {
            return (other.position - position).normalized;
        }

        public void RotateTowards(Vector3 direction, float rotationValue) {
            rotation = Quat.Slerp(Quat.Euler(rotation), Quat.LookRotation(direction), rotationValue).eulerAngles;
        }

        public float angleBetweenForwards(NebulaTransform other) {
            Vector3 myForward = Quat.Euler(rotation) * new Vector3(0, 0, 1);
            Vector3 otherForward = Quat.Euler(other.rotation) * new Vector3(0, 0, 1);
            var crossLen = myForward.Cross(otherForward).Length;
            var dotVal = myForward.Dot(otherForward);
            return Mathf.Atan2(crossLen, dotVal);
        }

        public float angleWithDirection(Vector3 direction) {
            Vector3 myForward = Quat.Euler(rotation) * new Vector3(0, 0, 1);
            Vector3 otherForward = direction;
            var crossLen = myForward.Cross(otherForward).Length;
            var dotVal = myForward.Dot(otherForward);
            return Mathf.Atan2(crossLen, dotVal);
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Transform;
            }
        }
    }
}
