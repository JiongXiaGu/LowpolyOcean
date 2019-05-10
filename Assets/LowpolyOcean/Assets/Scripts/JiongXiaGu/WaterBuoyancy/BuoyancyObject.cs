using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.WaterBuoyancy
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class BuoyancyObject : MonoBehaviour
    {
        protected BuoyancyObject()
        {
        }

        [SerializeField] private BuoyancyData buoyancyData;
        public BuoyancyData BuoyancyData
        {
            get { return buoyancyData; }
            set { buoyancyData = value; }
        }

        [SerializeField] private float power = 100f;
        public float Power
        {
            get { return power; }
            set { power = value; }
        }

        [SerializeField] private float drag = 0.02f;
        public float Drag
        {
            get { return drag; }
            set { drag = value; }
        }

        [SerializeField] private float angleDrag = 2f;
        public float AngleDrag
        {
            get { return angleDrag; }
            set { angleDrag = value; }
        }

        [HideInInspector] [SerializeField] private float originalDrag = 0f;
        [HideInInspector] [SerializeField] private float originalAngleDrag = 0f;
        [HideInInspector] [SerializeField] private bool isBackups = false;

        public virtual bool IsRequestUpdate => buoyancyData != null;
        protected Rigidbody _rigidbody;

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {
            if (buoyancyData != null)
            {
                buoyancyData.Collect();
            }
        }

        protected virtual void OnEnable()
        {
            BuoyancyManager.Subscribe(this);
            if (!isBackups)
            {
                originalDrag = _rigidbody.drag;
                originalAngleDrag = _rigidbody.angularDrag;
                isBackups = true;
            }
        }

        protected virtual void OnDisable()
        {
            BuoyancyManager.Unsubscribe(this);
            _rigidbody.drag = originalDrag;
            _rigidbody.angularDrag = originalAngleDrag;
            isBackups = false;
        }

        public void AddForce(PosAndForce force)
        {
            _rigidbody.AddForceAtPosition(force.Force, force.Position, ForceMode.Force);
        }

        protected virtual void UpdateRigidbodyState(int influenceDragCount, int activatedDrag)
        {
            if (buoyancyData.Handles.Count == 0)
                return;

            var scale = activatedDrag / buoyancyData.Handles.Count;
            _rigidbody.drag = Mathf.Max(Drag * scale, originalDrag);
            _rigidbody.angularDrag = Mathf.Max(AngleDrag * scale, originalAngleDrag);
        }

        public virtual void ManualUpdate(IOceanData data)
        {
            int influenceDragCount = 0;
            int activatedDrag = 0;
            foreach (var item in buoyancyData.Handles)
            {
                PosAndForce force;
                bool isAddForce = item.TryGetForce(data, this, out force);

                if (isAddForce)
                {
                    AddForce(force);

                    if (item.IsInfluenceDrag)
                    {
                        influenceDragCount++;
                        activatedDrag++;
                    }
                }
                else
                {
                    if (item.IsInfluenceDrag)
                    {
                        influenceDragCount++;
                    }
                }
            }

            UpdateRigidbodyState(influenceDragCount, activatedDrag);
        }
    }
}
