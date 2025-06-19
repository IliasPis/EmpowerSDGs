using System.Collections;
using UnityEngine;

namespace DiceRollLogic
{
    public class PawnMover : MonoBehaviour
    {
        public float moveDuration = 0.3f;
        public float cornerDistance = 5.8f;
        public float regularDistance = 3.8f;
        public AudioSource pawnMoveAudio;
        public System.Action<Vector3> onPawnTurn;

        private LayerMask playerLayer;
        private int currentStep = 0;
        private Vector3 forwardDirection = Vector3.forward;

        private void Start()
        {
            playerLayer = LayerMask.GetMask("Player");
        }

        public IEnumerator MovePawn(int diceValue)
        {
            // Execute the dice-specified steps first
            for (int i = 0; i < diceValue; i++)
            {
                currentStep++;
                float moveDistance = GetMoveDistance();

                if (pawnMoveAudio != null && !pawnMoveAudio.isPlaying)
                {
                    pawnMoveAudio.Play();
                }

                // Move to the next position
                Vector3 nextPosition = transform.position + forwardDirection * moveDistance;
                yield return MoveToPosition(nextPosition);

                // Turn at corners if required
                if (IsOnCorner())
                {
                    TurnAtCorner();
                }

                yield return new WaitForSeconds(0.2f);
            }

            // Now handle extra steps for finding a NonVisited block
            while (IsCurrentBlockVisited() || IsOnCorner())
            {
                Debug.Log("Continuing extra steps to find NonVisited block.");

                currentStep++;
                float moveDistance = GetMoveDistance();
                Vector3 nextPosition = transform.position + forwardDirection * moveDistance;
                yield return MoveToPosition(nextPosition);

                // Turn at corners but do not stop
                if (IsOnCorner())
                {
                    TurnAtCorner();
                }

                yield return new WaitForSeconds(0.2f);
            }

            // Stop the movement sound when completed
            if (pawnMoveAudio != null && pawnMoveAudio.isPlaying)
            {
                pawnMoveAudio.Stop();
            }
        }

        private float GetMoveDistance()
        {
            return (currentStep % 7 == 0 || (currentStep - 1) % 7 == 0) ? cornerDistance : regularDistance;
        }

        private IEnumerator MoveToPosition(Vector3 position)
        {
            float elapsedTime = 0f;
            Vector3 startPos = transform.position;

            while (elapsedTime < moveDuration)
            {
                transform.position = Vector3.Lerp(startPos, position, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = position;
        }

        private bool IsCurrentBlockVisited()
        {
            BlockComplete blockComplete = GetCurrentBlockComponent();
            return blockComplete != null && blockComplete.isVisited;
        }

        private BlockComplete GetCurrentBlockComponent()
        {
            RaycastHit hit;
            Vector3 rayOrigin = transform.position + Vector3.up * 1.0f;

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 3.0f, playerLayer))
            {
                return hit.collider.GetComponent<BlockComplete>();
            }

            return null;
        }

        private void TurnAtCorner()
        {
            forwardDirection = Quaternion.Euler(0, -90, 0) * forwardDirection;
            onPawnTurn?.Invoke(forwardDirection);
            Debug.Log("Turning at corner step " + currentStep);
        }

        public bool IsOnCorner()
        {
            return currentStep % 7 == 0;
        }
    }
}
