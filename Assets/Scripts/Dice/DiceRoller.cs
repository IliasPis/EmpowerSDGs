using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

namespace DiceRollLogic
{
    public class DiceRoller : MonoBehaviour
    {
        public Rigidbody diceRigidbody;
        public Transform pawnTransform;
        public CinemachineVirtualCamera cinemachineCamera;
        public Button rollButton;
        public AudioSource diceRollAudio;
        public AudioSource numberDisplayAudio;

        public float zoomFieldOfView = 35f; // Reduced zoom for dice focus
        public float postQuizZoomFieldOfView = 45f; // Adjustable zoom after quiz
        public float pawnFollowFieldOfView = 60f; // Adjustable view for pawn follow
        public float quizZoomOutFieldOfView = 70f; // Adjustable view for quiz zoom out

        public float transitionDuration = 1f;
        public float quizZoomDuration = 2f;

        public GameObject diceNumberDisplayObject;
        public GameObject[] numberImages;
        public GameObject face1;
        public GameObject face2;
        public GameObject face3;
        public GameObject face4;
        public GameObject face5;
        public GameObject face6;
        public GameObject OpenQuiz;
        public GameObject PopUpQuiz;
        public GameObject finalQuizCompletionObject;

        public Color visitedColor = Color.red;
        public string playerTag = "Player";

        private bool isRolling = false;
        private float originalFieldOfView;
        private Vector3 initialCameraPosition;
        private Quaternion initialCameraRotation;
        private bool isDiceRolling = false;
        private bool quizOpen = false;

        private HashSet<GameObject> visitedBlocks = new HashSet<GameObject>();
        private bool shouldSkipBlock = false;

        public Vector3 cameraOffset; // Single offset for all sides

        private PawnMover pawnMover;
        private int currentCameraSide = 0;

        private readonly List<Vector3> cameraPositions = new List<Vector3>
        {
            new Vector3(36.9f, 24f, -3.84f), // 1st side
            new Vector3(6.8f, 25.9f, 26.79f), // 2nd side
            new Vector3(-23.66f, 25.9f, -2.5f), // 3rd side
            new Vector3(6.78f, 24.25f, -30.18f) // 4th side
        };

        private readonly List<Quaternion> cameraRotations = new List<Quaternion>
        {
            Quaternion.Euler(33.8f, -85.2f, 4.074f), // 1st side
            Quaternion.Euler(499.49f, 1.192001f, 180.844f), // 2nd side
            Quaternion.Euler(34.1f, 87.411f, -2.245f), // 3rd side
            Quaternion.Euler(36.351f, 0f, 0.264f) // 4th side
        };

        void Start()
        {
            rollButton.onClick.AddListener(RollDice);
            originalFieldOfView = cinemachineCamera.m_Lens.FieldOfView;
            initialCameraPosition = cinemachineCamera.transform.position;
            initialCameraRotation = cinemachineCamera.transform.rotation;
            diceNumberDisplayObject.SetActive(false);

            if (pawnTransform != null)
            {
                pawnMover = pawnTransform.GetComponent<PawnMover>();
                if (pawnMover == null)
                {
                    Debug.LogError("PawnMover component missing on pawnTransform. Please attach the PawnMover script to the pawn object.");
                }
                else
                {
                    pawnMover.onPawnTurn += OnPawnTurn;
                }
            }
            else
            {
                Debug.LogError("pawnTransform is not assigned in DiceRoller. Please set it in the inspector.");
            }
        }

        void Update()
        {
            if (quizOpen && PopUpQuiz != null && !PopUpQuiz.activeSelf)
            {
                StartCoroutine(SmoothZoomInTransition());
                quizOpen = false;

                if (finalQuizCompletionObject != null && visitedBlocks.Count == 24)
                {
                    finalQuizCompletionObject.SetActive(true);
                }
            }
        }

        public void RegisterVisitedBlock(GameObject block)
        {
            if (!visitedBlocks.Contains(block))
            {
                visitedBlocks.Add(block);
                ChangeBlockColor(block.transform, visitedColor);
            }
        }

        public void HandleVisitedBlock()
        {
            Debug.Log("HandleVisitedBlock called. Skipping block.");
            shouldSkipBlock = true;
        }

        void RollDice()
        {
            if (!isRolling)
            {
                rollButton.gameObject.SetActive(false);
                isRolling = true;
                StartCoroutine(FocusOnPawnThenRoll());
            }
        }

        IEnumerator FocusOnPawnThenRoll()
        {
            yield return StartCoroutine(SmoothTransition(pawnTransform, pawnFollowFieldOfView));

            diceRigidbody.AddForce(Random.Range(-5, 5), Random.Range(5, 10), Random.Range(-5, 5), ForceMode.Impulse);
            diceRigidbody.AddTorque(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10), ForceMode.Impulse);
            StartCoroutine(HandleDiceSound());
            yield return StartCoroutine(FollowSequence());
        }

        IEnumerator HandleDiceSound()
        {
            yield return new WaitUntil(() => !diceRigidbody.IsSleeping());
            if (diceRollAudio != null && !diceRollAudio.isPlaying)
            {
                diceRollAudio.Play();
                isDiceRolling = true;
            }
            yield return new WaitUntil(() => diceRigidbody.IsSleeping());
            if (diceRollAudio != null && diceRollAudio.isPlaying)
            {
                diceRollAudio.Stop();
                isDiceRolling = false;
            }
        }

        IEnumerator FollowSequence()
        {
            yield return new WaitUntil(() => diceRigidbody.IsSleeping());
            int diceValue = GetDiceTopFaceValue();
            yield return StartCoroutine(ShowDiceResult(diceValue));
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(SmoothPawnFollowTransition());

            if (pawnMover != null)
            {
                yield return pawnMover.MovePawn(diceValue);

                if (pawnMover.IsOnCorner())
                {
                    rollButton.gameObject.SetActive(true);
                    isRolling = false;
                    if (OpenQuiz != null)
                    {
                        OpenQuiz.SetActive(false);
                    }
                }
                else
                {
                    StartCoroutine(SmoothOpenQuizTransition());
                }
            }
            else
            {
                Debug.LogError("pawnMover is null in FollowSequence. Please ensure PawnMover is attached to pawnTransform.");
                isRolling = false;
            }
        }

        int GetDiceTopFaceValue()
        {
            float maxHeight = Mathf.Max(face1.transform.position.y, face2.transform.position.y, face3.transform.position.y, face4.transform.position.y, face5.transform.position.y, face6.transform.position.y);
            if (maxHeight == face1.transform.position.y) return 1;
            if (maxHeight == face2.transform.position.y) return 2;
            if (maxHeight == face3.transform.position.y) return 3;
            if (maxHeight == face4.transform.position.y) return 4;
            if (maxHeight == face5.transform.position.y) return 5;
            return 6;
        }

        IEnumerator ShowDiceResult(int diceValue)
        {
            diceNumberDisplayObject.SetActive(true);
            if (numberDisplayAudio != null) numberDisplayAudio.Play();
            for (int i = 0; i < numberImages.Length; i++)
                numberImages[i].SetActive(i == (diceValue - 1));
            yield return new WaitForSeconds(3f);
            diceNumberDisplayObject.SetActive(false);
        }

        void ChangeBlockColor(Transform block, Color color)
        {
            Renderer renderer = block.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }

        IEnumerator SmoothTransition(Transform target, float targetFieldOfView)
        {
            cinemachineCamera.Follow = target;
            float elapsedTime = 0f;
            float startFieldOfView = cinemachineCamera.m_Lens.FieldOfView;
            Vector3 startPos = cinemachineCamera.transform.position;
            Quaternion startRot = cinemachineCamera.transform.rotation;

            while (elapsedTime < transitionDuration)
            {
                float t = Mathf.SmoothStep(0f, 1f, elapsedTime / transitionDuration);
                cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(startFieldOfView, targetFieldOfView, t);
                cinemachineCamera.transform.position = Vector3.Lerp(startPos, target.position + cameraOffset, t);
                cinemachineCamera.transform.rotation = Quaternion.Lerp(startRot, Quaternion.LookRotation(target.position - cinemachineCamera.transform.position), t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            cinemachineCamera.m_Lens.FieldOfView = targetFieldOfView;
        }

        IEnumerator SmoothPawnFollowTransition()
        {
            Vector3 targetPos = cameraPositions[currentCameraSide] + cameraOffset;
            Quaternion targetRot = cameraRotations[currentCameraSide];
            float elapsedTime = 0f;
            float startFieldOfView = cinemachineCamera.m_Lens.FieldOfView;
            Vector3 startPos = cinemachineCamera.transform.position;
            Quaternion startRot = cinemachineCamera.transform.rotation;

            while (elapsedTime < transitionDuration)
            {
                float t = Mathf.SmoothStep(0f, 1f, elapsedTime / transitionDuration);
                cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(startFieldOfView, pawnFollowFieldOfView, t);
                cinemachineCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
                cinemachineCamera.transform.rotation = Quaternion.Lerp(startRot, targetRot, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            cinemachineCamera.m_Lens.FieldOfView = pawnFollowFieldOfView;
            cinemachineCamera.transform.position = targetPos;
            cinemachineCamera.transform.rotation = targetRot;
        }

        IEnumerator SmoothOpenQuizTransition()
        {
            float elapsedTime = 0f;
            float startFieldOfView = cinemachineCamera.m_Lens.FieldOfView;

            while (elapsedTime < quizZoomDuration)
            {
                float t = Mathf.SmoothStep(0f, 1f, elapsedTime / quizZoomDuration);
                cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(startFieldOfView, quizZoomOutFieldOfView, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            cinemachineCamera.m_Lens.FieldOfView = quizZoomOutFieldOfView;
            OpenQuiz.SetActive(true);
            rollButton.gameObject.SetActive(true);
            quizOpen = true;
        }

        IEnumerator SmoothZoomInTransition()
        {
            float elapsedTime = 0f;
            float startFieldOfView = cinemachineCamera.m_Lens.FieldOfView;

            while (elapsedTime < quizZoomDuration)
            {
                float t = Mathf.SmoothStep(0f, 1f, elapsedTime / quizZoomDuration);
                cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(startFieldOfView, postQuizZoomFieldOfView, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            cinemachineCamera.m_Lens.FieldOfView = postQuizZoomFieldOfView;

            // Reset states to allow new dice rolls
            isRolling = false;
            rollButton.gameObject.SetActive(true);
        }

        void OnPawnTurn(Vector3 newForwardDirection)
        {
            currentCameraSide = (currentCameraSide + 1) % cameraPositions.Count;
            StartCoroutine(SmoothPawnFollowTransition());
        }
    }
}
