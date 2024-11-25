using UnityEngine;
using UnityEngine.AI;

namespace KikiNgao.SimpleBikeControl
{
    public class SimpleBike : MonoBehaviour
    {
        [Tooltip("Control without biker")]
        public bool noBikerCtrl;
        public Transform bikerHolder;

        public WheelCollider frontWheelCollider;
        public WheelCollider rearWheelCollider;
        public GameObject frontWheel;
        public GameObject rearWheel;

        public Transform handlerBar;
        public Transform cranksetTransform;

        [SerializeField] private float legPower = 10;
        [Tooltip("speed multiply max")]
        [SerializeField] private float powerUpMax = 2;
        [Tooltip("define how fast to reach power up max")]
        [SerializeField] private float powerUpSpeed = .5f;
        [Tooltip("Rigidbody Drag while standing")]
        [SerializeField] private float restDrag = 2f;
        [Tooltip("Rigidbody AngularDrag while standing")]
        [SerializeField] private float restAngularDrag = .2f;
        [Tooltip("ratio of wheels and crankset rotation ")]
        // Ex:forceRatio = 2f; mean when crankset rotate 1 round the wheels rotate 2 round
        [SerializeField] private float forceRatio = 2f;
        // default: speed [0 -> 50] Angle [1 -> 35]
        [SerializeField] private AnimationCurve frontWheelRestrictCurve = new AnimationCurve(new Keyframe(0f, 35f), new Keyframe(50f, 1f));

        public Transform leftHandTarget, rightHandTarget;
        public Transform leftPendalTarget, rightPendalTarget;
        public Transform leftStandTarget, rightStandTarget;

        private Transform centerOfMass;

        private Rigidbody m_Rigidbody;
        public Rigidbody GetRigidbody() => m_Rigidbody;


        [HideInInspector]
        public bool falling;
        private float fallingDrag = 1;
        private float fallingAngurlarDrag = 0.01f;

        private float temporaryFrontWheelAngle;

        private float handlerBarYLastAngle;
        private float currentLegPower;
        private float reversePower;
        private EventManager eventManager;

        private float GetBikeSpeedKm() => GetBikeSpeedMs() * 3.6f;
        private float GetBikeSpeedMs() => m_Rigidbody.velocity.magnitude;
        private float GetBikeAngle() => WrapAngle(transform.eulerAngles.z);

        public bool TiltToRight() => WrapAngle(transform.eulerAngles.z) <= 0;

        public bool Freeze { get => m_Rigidbody.isKinematic; set => m_Rigidbody.isKinematic = value; }
        public bool FreezeCrankset { get; set; }
        public bool ReadyToRide()
        {
            if (noBikerCtrl) return true;
            if (bikerHolder.childCount == 0) return false;
            if (bikerHolder.GetChild(0).CompareTag("Player")) return true;
            return false;
        }

        void Start()
        {

            CreateCenterOfMass();
            SettingRigidbody();

            currentLegPower = legPower * 10;
            reversePower = legPower * 3;

            Freeze = true;

        }

        //creat cemter of mass and add to the bike
        private void CreateCenterOfMass()
        {
            centerOfMass = new GameObject().transform;
            centerOfMass.name = "CenterOfMass";
            Vector3 center = new Vector3();
            Vector3 rearPosition = rearWheelCollider.transform.position;
            center.x = rearPosition.x;
            center.y = 0;
            center.z = rearPosition.z + (frontWheelCollider.transform.position.z - rearPosition.z) / 2;

            centerOfMass.transform.position = center;
            centerOfMass.parent = transform;
        }
        private void SettingRigidbody()
        {
            m_Rigidbody = transform.GetComponent<Rigidbody>();
            m_Rigidbody.centerOfMass = centerOfMass.transform.position;
        }
        float powerUp = 1f;
        private void FixedUpdate()
        {
            // falling when exit bike 
            if (falling) { Falling(); return; };

            if (GetComponent<NavMeshAgent>().velocity.magnitude > 0f)
            {
                MovingBike();
            }
            else
            {
                Rest();
            }
            if (!FreezeCrankset) UpdateCranksetRotation();
            UpdateWheelDisplay();
        }

        private void UpdateLegPower(bool speedUp)
        {
            if (speedUp)
            {
                powerUp += powerUpSpeed * Time.deltaTime;
                if (powerUp >= powerUpMax) powerUp = powerUpMax;
                currentLegPower = legPower * 10 * powerUp;

                eventManager?.OnSpeedUp();

                return;
                //Debug.Log(powerUp);
            }
            eventManager?.OnNormalSpeed();
            powerUp = 1f;
            currentLegPower = legPower * 10 * powerUp;
        }
        private void MovingBike()
        {
            Freeze = false;
            m_Rigidbody.drag = 0.5f;
            m_Rigidbody.angularDrag = 5.5f;

            frontWheelCollider.brakeTorque = 0;
            rearWheelCollider.motorTorque =  reversePower;

            UpdateCenterOfMass();
        }


        // the bike keep moving slowly if we forgot reset motor force
        private void ResetWheelsCollider()
        {
            frontWheelCollider.steerAngle = 0f;
            frontWheelCollider.motorTorque = 0;
            rearWheelCollider.motorTorque = 0;
            rearWheelCollider.brakeTorque = 0;
            frontWheelCollider.brakeTorque = 0;
        }

        // when had biker and no key press
        private void Rest()
        {
            // the bike auto stop due to high drag
            Freeze = true;
            m_Rigidbody.drag = restDrag;
            m_Rigidbody.angularDrag = restAngularDrag;
            ResetWheelsCollider();
            UpdateCenterOfMass();
        }
        //When biker exit bike it's start falling 
        public void Falling()
        {
            falling = true;
            // the bike auto stop due to high drag
            m_Rigidbody.drag = fallingDrag;
            m_Rigidbody.angularDrag = fallingAngurlarDrag;

            UpdateCenterOfMass();
            UpdateWheelDisplay();
            ResetWheelsCollider();

            //Debug.Log(gameObject.name + "  Falling ");
            float angle = GetBikeAngle();
            if (angle < -75 || angle > 75) { Freeze = true; falling = false; }
        }
        private void UpdateCranksetRotation()
        {
            cranksetTransform.rotation *= Quaternion.Euler(GetBikeSpeedKm() / forceRatio, 0, 0);
            Quaternion ro = Quaternion.Euler(-GetBikeSpeedKm() / forceRatio, 0, 0);
            cranksetTransform.GetChild(0).rotation *= ro;
            cranksetTransform.GetChild(1).rotation *= ro;
        }

        private void UpdateWheelDisplay()
        {
            Vector3 temporaryVector;
            Quaternion temporaryQuaternion;

            rearWheelCollider.GetWorldPose(out temporaryVector, out temporaryQuaternion);
            rearWheel.transform.position = temporaryVector;

            Quaternion rearWheelRot = rearWheel.transform.rotation;

            rearWheel.transform.rotation = rearWheelRot * Quaternion.Euler(GetBikeSpeedKm(), 0, 0);

            frontWheel.transform.localRotation = rearWheel.transform.localRotation;
        }

        private void UpdateCenterOfMass()
        {
            var centerLocal = centerOfMass.localPosition;

            if (!falling)
            {
                centerLocal.y = -0.8f; //set center local y under ground to make bike tilt while riding 
            }
            else //falling
            {
                centerLocal.y = 0;
                centerLocal.x = TiltToRight() ? .2f : -0.2f;
            }

            // when bike falling
            m_Rigidbody.centerOfMass = centerLocal;
        }

        private bool OnGround(WheelCollider wheelCollider)
        {
            if (Physics.Raycast(wheelCollider.transform.position, -transform.up, out RaycastHit hit, wheelCollider.radius + 0.1f))
            {
                return true;
            }
            return false;
        }
        // convert runtime Euler Angle to angle that showing in Unity Editor 
        private static float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }

    }
}
