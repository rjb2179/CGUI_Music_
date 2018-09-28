using UnityEngine;

namespace Leap.Unity
{

    /// <summary>
    /// A basic utility class to aid in creating pinch based actions.  Once linked with an IHandModel, it can
    /// be used to detect pinch gestures that the hand makes.
    /// </summary>
    public class MyPinchDetector : MyDetector
    {

        protected const float MM_TO_M = 0.001f;

        public bool leftHand;

        public Leap.Unity.LeapXRServiceProvider provider;

        [SerializeField]
        protected float _activatePinchDist = 0.03f;

        [SerializeField]
        protected float _deactivatePinchDist = 0.05f;

        protected bool _isPinching = false;
        protected bool _didChange = false;

        protected float _lastPinchTime = 0.0f;
        protected float _lastUnpinchTime = 0.0f;

        protected Vector3 _pinchPos;
        protected Quaternion _pinchRotation;
        protected Quaternion _lastPinchRotation;

        private Frame currentFrame;

        protected virtual void OnValidate()
        {

            _activatePinchDist = Mathf.Max(0, _activatePinchDist);
            _deactivatePinchDist = Mathf.Max(0, _deactivatePinchDist);

            // Activate distance cannot be greater than deactivate distance
            if (_activatePinchDist > _deactivatePinchDist)
            {
                _deactivatePinchDist = _activatePinchDist;
            }
        }


        protected virtual void Update()
        {
            _lastPinchRotation = _pinchRotation;

            // We ensure the data is up to date at all times because
            // there are some values (like LastPinchTime) that cannot
            // be updated on demand
            ensurePinchInfoUpToDate();
        }

        /// <summary>
        /// Returns whether or not the dectector is currently detecting a pinch.
        /// </summary>
        public bool IsPinching
        {
            get
            {
                // ensurePinchInfoUpToDate();
                return _isPinching;
            }
        }

        /// <summary>
        /// Returns whether or not the value of IsPinching is different than the value reported during
        /// the previous frame.
        /// </summary>
        public bool DidChangeFromLastFrame
        {
            get
            {
                // ensurePinchInfoUpToDate();
                return _didChange;
            }
        }

        /// <summary>
        /// Returns whether or not the value of IsPinching changed to true between this frame and the previous.
        /// </summary>
        public bool DidStartPinch
        {
            get
            {
                // ensurePinchInfoUpToDate();
                return DidChangeFromLastFrame && IsPinching;
            }
        }

        /// <summary>
        /// Returns whether or not the value of IsPinching changed to false between this frame and the previous.
        /// </summary>
        public bool DidEndPinch
        {
            get
            {
                // ensurePinchInfoUpToDate();
                return DidChangeFromLastFrame && !IsPinching;
            }
        }

        /// <summary>
        /// Returns the value of Time.time during the most recent pinch event.
        /// </summary>
        public float LastPinchTime
        {
            get
            {
                // ensurePinchInfoUpToDate();
                return _lastPinchTime;
            }
        }

        /// <summary>
        /// Returns the value of Time.time during the most recent unpinch event.
        /// </summary>
        public float LastUnpinchTime
        {
            get
            {
                // ensurePinchInfoUpToDate();
                return _lastUnpinchTime;
            }
        }

        /// <summary>
        /// Returns the position value of the detected pinch.  If a pinch is not currently being
        /// detected, returns the most recent pinch position value.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                // ensurePinchInfoUpToDate();
                return _pinchPos;
            }
        }

        /// <summary>
        /// Returns the rotation value of the detected pinch.  If a pinch is not currently being
        /// detected, returns the most recent pinch rotation value.
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                // ensurePinchInfoUpToDate();
                return _pinchRotation;
            }
        }

        public Quaternion LastRotation
        {
            get
            {
                // ensurePinchInfoUpToDate();
                return _lastPinchRotation;
            }
        }

        protected virtual void ensurePinchInfoUpToDate()
        {

            _didChange = false;

            currentFrame = provider.CurrentFrame;

            Hand hand = null;

            if (leftHand)
            {
                foreach (Hand h in currentFrame.Hands)
                {
                    if (h.IsLeft)
                    {
                        hand = h;
                        break;
                    }
                }
            }
            else
            {
                foreach (Hand h in currentFrame.Hands)
                {
                    if (!h.IsLeft)
                    {
                        hand = h;
                        break;
                    }
                }
            }

            if (hand == null)
            {
                changePinchState(false);
                return;
            }

            float pinchDistance = hand.PinchDistance * MM_TO_M;
            transform.rotation = hand.Basis.CalculateRotation();

            var fingers = hand.Fingers;
            transform.position = Vector3.zero;
            for (int i = 0; i < fingers.Count; i++)
            {
                Finger finger = fingers[i];
                if (finger.Type == Finger.FingerType.TYPE_INDEX ||
                    finger.Type == Finger.FingerType.TYPE_THUMB)
                {
                    transform.position += finger.Bone(Bone.BoneType.TYPE_DISTAL).NextJoint.ToVector3();
                }
            }
            transform.position /= 2.0f;

            if (_isPinching)
            {
                if (pinchDistance > _deactivatePinchDist)
                {
                    changePinchState(false);
                    return;
                }
            }
            else
            {
                if (pinchDistance < _activatePinchDist)
                {
                    changePinchState(true);
                }
            }

            if (_isPinching)
            {
                _pinchPos = transform.position;
                _pinchRotation = transform.rotation;
            }
        }

        protected virtual void changePinchState(bool shouldBePinching)
        {
            if (_isPinching != shouldBePinching)
            {
                _isPinching = shouldBePinching;

                if (_isPinching)
                {
                    _lastPinchTime = Time.time;
                    Activate();
                }
                else
                {
                    _lastUnpinchTime = Time.time;
                    Deactivate();
                }

                _didChange = true;
            }
        }
    }
}
