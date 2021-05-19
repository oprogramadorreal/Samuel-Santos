using UnityEngine;

namespace ss
{
    /// <summary>
    /// Based on https://youtu.be/uVtDyPmorA0
    /// </summary>
    public sealed class AnimationController : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;

        private const float runningThreshold = 0.75f;

        private int walkRightHash;
        private int walkLeftHash;
        private int runRightHash;
        private int runLeftHash;
        private int matrixLeftHash;
        private int matrixRightHash;
        private int jumpHash;
        private int crouchHash;
        private int idleHash;

        private void Start()
        {
            walkRightHash = Animator.StringToHash("WalkRight");
            walkLeftHash = Animator.StringToHash("WalkLeft");
            runRightHash = Animator.StringToHash("RunRight");
            runLeftHash = Animator.StringToHash("RunLeft");
            matrixLeftHash = Animator.StringToHash("MatrixLeft");
            matrixRightHash = Animator.StringToHash("MatrixRight");
            jumpHash = Animator.StringToHash("Jump");
            crouchHash = Animator.StringToHash("Crouch");
            idleHash = Animator.StringToHash("Idle");
        }

        public void StayIdle()
        {
            animator.SetTrigger(idleHash);
        }

        public void Crouch()
        {
            animator.Play(crouchHash);
        }

        public void Jump()
        {
            animator.Play(jumpHash);
        }

        public void MoveLeft(float amount)
        {
            if (amount < runningThreshold)
            {
                animator.Play(walkLeftHash);
            }
            else
            {
                animator.Play(runLeftHash);
            }
        }

        public void MoveRight(float amount)
        {
            if (amount < runningThreshold)
            {
                animator.Play(walkRightHash);
            }
            else
            {
                animator.Play(runRightHash);
            }
        }

        public void MatrixLeft()
        {
            animator.Play(matrixLeftHash);
        }

        public void MatrixRight()
        {
            animator.Play(matrixRightHash);
        }
    }
}
