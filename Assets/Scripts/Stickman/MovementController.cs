using System;
using UnityEngine;

namespace ss
{
    public sealed class MovementController : MonoBehaviour
    {
        [SerializeField]
        private AnimationController animationController;

        [SerializeField]
        private Rigidbody2D body;

        [SerializeField]
        private float speed = 2000;

        [SerializeField]
        private ArmRotator[] armsRotators;

        private Action[] bodyActions;

        private Vector2? bodyInputAxis;
        private Vector2? armsInputAxis;

        private StickmanAction currentBodyAction;

        public Vector2 BodyPosition { get => body.position; }

        public void SetInputAxisForBodyMovement(Vector2 value)
        {
            bodyInputAxis = value;
        }

        public void SetInputAxisForArmsMovement(Vector2 value)
        {
            armsInputAxis = value;
        }

        public bool CheckMatrixPose()
        {
            return currentBodyAction == StickmanAction.MatrixLeft
                || currentBodyAction == StickmanAction.MatrixRight;
        }

        private void Awake()
        {
            bodyActions = new Action[Enum.GetNames(typeof(StickmanAction)).Length];
            bodyActions[(int)StickmanAction.None] = StayIdle;
            bodyActions[(int)StickmanAction.Jump] = Jump;
            bodyActions[(int)StickmanAction.Crouch] = Crouch;
            bodyActions[(int)StickmanAction.MoveLeft] = MoveLeft;
            bodyActions[(int)StickmanAction.MoveRight] = MoveRight;
            bodyActions[(int)StickmanAction.MatrixLeft] = MatrixLeft;
            bodyActions[(int)StickmanAction.MatrixRight] = MatrixRight;
        }

        private void LateUpdate()
        {
            if (bodyInputAxis.HasValue)
            {
                UpdateBodyMovement(bodyInputAxis.Value);
                bodyInputAxis = null;
            }
            else
            {
                bodyInputAxis = Vector2.zero; // next frame StickmanAction.None will be generated
            }

            if (armsInputAxis.HasValue)
            {
                UpdateArmsMovement(armsInputAxis.Value);
                armsInputAxis = null;
            }
        }

        private void UpdateBodyMovement(Vector2 inputDirection)
        {
            currentBodyAction = GetAction(inputDirection);
            bodyActions[(int)currentBodyAction].Invoke();
        }

        private void UpdateArmsMovement(Vector2 inputDirection)
        {
            var inputDirectionNorm = inputDirection.normalized;
            var inputForce = inputDirection.magnitude;

            foreach (var r in armsRotators)
            {
                if (r != null)
                {
                    r.RotateToDirection(inputDirectionNorm, inputForce);
                }
            }
        }

        private void StayIdle()
        {
            animationController.StayIdle();
        }

        private void Crouch()
        {
            animationController.Crouch();
        }

        private void Jump()
        {
            animationController.Jump();
        }

        private void MoveLeft()
        {
            var amount = Mathf.Abs(bodyInputAxis.Value.x);
            animationController.MoveLeft(amount);
            body.AddForce(Vector2.left * speed * amount * Time.deltaTime);
        }

        private void MoveRight()
        {
            var amount = Mathf.Abs(bodyInputAxis.Value.x);
            animationController.MoveRight(amount);
            body.AddForce(Vector2.right * speed * amount * Time.deltaTime);
        }

        private void MatrixLeft()
        {
            animationController.MatrixLeft();
        }

        private void MatrixRight()
        {
            animationController.MatrixRight();
        }

        private static StickmanAction GetAction(Vector2 inputDirection)
        {
            var inputDirectionNorm = inputDirection.normalized;
            var inputForce = inputDirection.magnitude;
            var inputAngle = Vector2.Angle(inputDirectionNorm, Vector2.up);

            return GetAction(inputDirection, inputForce, inputAngle);
        }

        private static StickmanAction GetAction(Vector2 inputDirection, float inputForce, float inputAngle)
        {
            const float dirActionThreshold = 0.8f;

            if (inputForce > dirActionThreshold)
            {
                if (inputAngle < 30.0f) // 0 to 30
                {
                    return StickmanAction.Jump;
                }
                else if (inputAngle < 120.0f) // 30 to 120
                {
                    // run
                }
                else if (inputAngle < 170.0f) // 120 to 170
                {
                    if (inputDirection.x < 0.0f)
                    {
                        return StickmanAction.MatrixLeft;
                    }
                }
                else // 170 to 180
                {
                    return StickmanAction.Crouch;
                }
            }

            if (inputDirection.x > 0)
            {
                return StickmanAction.MoveRight;
            }
            else if (inputDirection.x < 0)
            {
                return StickmanAction.MoveLeft;
            }

            return StickmanAction.None;
        }

        private enum StickmanAction
        {
            None = 0,
            Jump,
            Crouch,
            MoveLeft,
            MoveRight,
            MatrixLeft,
            MatrixRight
        }
    }
}
