using UnityEngine;
using System.Collections;

public class NetworkTransformInterpolation : MonoBehaviour {

    public enum InterpolationMode {
        INTERPOLATION,
        EXTRAPOLATION,
        CUSTOM
    }

    private InterpolationMode mode = InterpolationMode.CUSTOM;
    private float interpolationBackTime = 0.05f;
    private float extrapolationForwardTime = 2.0f;//1.5f;

    ExtrapolationData[] bufferedStates = new ExtrapolationData[20];
    int statesCount = 0;

    private float speed = 1.0f;
    private float _rotationSpeed = 1.0f;

    private ExtrapolationData _prev;
    private ExtrapolationData _current;

    bool Almost(Vector3 v1, Vector3 v2) {
        float eps = 1e-4f;
        float dx = Mathf.Abs(v1.x - v2.x);
        float dy = Mathf.Abs(v1.y - v2.y);
        float dz = Mathf.Abs(v1.z - v2.z);
        return dx < eps && dy < eps && dz < eps;
    }

    public void ReceivedData(ExtrapolationData data) {
        //print(string.Format("receive position: {0}", data.Position));

        if (bufferedStates[0] != null) {
            if (Almost(bufferedStates[0].Position, data.Position))
                return;
        }

        if (mode == InterpolationMode.CUSTOM) {
            if (_current != null) {
                if (Almost(_current.Position, data.Position) && Almost(_current.Rotation, data.Rotation))
                    return;
            }
            _prev = _current;
            _current = data;
        } else {
            //Vector3 pos = data.Position;
            //uaternion rot = Quaternion.Euler(data.Rotation);
            for (int i = bufferedStates.Length - 1; i >= 1; i--) {
                bufferedStates[i] = bufferedStates[i - 1];
            }
            bufferedStates[0] = data;
            statesCount = Mathf.Min(statesCount + 1, bufferedStates.Length);
            for (int i = 0; i < statesCount - 1; i++) {
                if (bufferedStates[i].Time < bufferedStates[i + 1].Time) {
                    print("<color=orange>State inconsistent</color>");
                }
            }
        }
    }

    //private Vector3 nDir = Vector3.zero;

    private bool CheckVector(Vector3 vec) {
        if (float.IsNaN(vec.x))
            return false;
        if (float.IsNaN(vec.y))
            return false;
        if (float.IsNaN(vec.z))
            return false;
        return true;
    }

    private bool AlmostZero(Vector3 vec) {
        return Mathf.Abs(vec.x) < 0.00001f && Mathf.Abs(vec.y) < 0.00001f && Mathf.Abs(vec.z) < 0.00001f;
    }
    void Update() {
        if (mode == InterpolationMode.CUSTOM) {

            if (_current != null && _prev == null) {
                transform.position = _current.Position;
                transform.rotation = Quaternion.Euler(_current.Rotation);
            } else if (_current != null && _prev != null) {
                Vector3 oldPos = transform.position;
                transform.position = Vector3.SmoothDamp(transform.position, _current.Position, ref smoothVel, extrapolationForwardTime);
                if (this.CheckVector(_current.Rotation)) {
                    Vector3 dir = (transform.position - oldPos).normalized;

                    if (!AlmostZero(dir)) {
                        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _rotationSpeed); //
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(_current.Rotation), Time.deltaTime * _rotationSpeed);
                    } else {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(_current.Rotation), Time.deltaTime * _rotationSpeed);
                    }
                }

            }
        } else {
            if (statesCount == 0)
                return;
            float currentTime = Time.time;
            float interpolationTime = currentTime - interpolationBackTime;

            if (mode == InterpolationMode.INTERPOLATION && bufferedStates[0].Time > interpolationTime) {
                for (int i = 0; i < statesCount; i++) {
                    if (bufferedStates[i].Time <= interpolationTime || i == statesCount - 1) {
                        var rhs = bufferedStates[Mathf.Max(i - 1, 0)];
                        var lhs = bufferedStates[i];
                        float length = rhs.Time - lhs.Time;
                        float t = 0.0f;
                        if (length > 0.0001f)
                            t = (interpolationTime - lhs.Time) / length;
                        transform.position = Vector3.Lerp(lhs.Position, rhs.Position, t);
                        transform.rotation = Quaternion.Slerp(Quaternion.Euler(lhs.Rotation), Quaternion.Euler(rhs.Rotation), t);
                    }
                }
            } else {
                float extrapolationLength = (currentTime - bufferedStates[0].Time);
                if (mode == InterpolationMode.EXTRAPOLATION /*&& extrapolationLength < extrapolationForwardTime*/ && statesCount > 1) {
                    Vector3 dif = bufferedStates[0].Position - bufferedStates[1].Position;
                    float distance = Vector3.Distance(bufferedStates[0].Position, bufferedStates[1].Position);
                    float timeDif = bufferedStates[0].Time - bufferedStates[1].Time;

                    if (Mathf.Approximately(distance, 0.0f) || Mathf.Approximately(timeDif, 0.0f)) {
                        transform.position = Vector3.SmoothDamp(transform.position, bufferedStates[0].Position, ref smoothVel, extrapolationForwardTime);
                        transform.rotation = Quaternion.Euler(bufferedStates[0].Rotation); //Quaternion.Slerp(transform.rotation, Quaternion.Euler(bufferedStates[0].Rotation), Time.deltaTime * _rotationSpeed);
                        return;
                    }
                    speed = distance / timeDif;
                    dif = dif.normalized;
                    Vector3 expectedPosition = bufferedStates[0].Position + dif * extrapolationLength * speed;
                    transform.position = Vector3.SmoothDamp(transform.position, expectedPosition, ref smoothVel2, Mathf.Min(timeDif, 1.0f)); //Vector3.Lerp(transform.position, expectedPosition, Time.deltaTime * speed);
                } else {
                    //transform.position = Vector3.Lerp(transform.position, bufferedStates[0].Position, Time.deltaTime * speed);
                    transform.position = bufferedStates[0].Position;

                }
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(bufferedStates[0].Rotation), Time.deltaTime * _rotationSpeed);
            }
        }

    }

    private Vector3 smoothVel;
    private Vector3 smoothVel2;
    private Vector3 smoothRotVel;
}

