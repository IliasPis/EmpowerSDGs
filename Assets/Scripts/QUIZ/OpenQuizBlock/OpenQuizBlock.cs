using System.Collections;
using UnityEngine;

namespace DiceRollLogic
{
    public class OpenQuizBlock : MonoBehaviour
    {
        public GameObject OpenQuiz;
        public GameObject PopUpQuiz;
        public GameObject CurrentSDG;
        public DiceRoller diceRollerMain;

        public float detectionRadius = 1.5f;
        public float requiredStayTime = 3f;
        public string playerTag = "Player";

        private bool isOn = true;
        private float stayTimer = 0f;
        private bool isCheckingSDG = false;

        private GameObject player;

        void Start()
        {
            if (OpenQuiz != null)
                OpenQuiz.SetActive(false);

            if (PopUpQuiz != null)
                PopUpQuiz.SetActive(false);

            player = GameObject.FindGameObjectWithTag(playerTag);
        }

        void Update()
        {
            if (player == null) return;

            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            if (distanceToPlayer <= detectionRadius && gameObject.tag != "Visited")
            {
                stayTimer += Time.deltaTime;

                if (isOn && stayTimer >= requiredStayTime)
                {
                    ActivateQuiz();

                    // Set the block to visited using BlockComplete
                    BlockComplete blockComplete = GetComponent<BlockComplete>();
                    if (blockComplete != null)
                    {
                        blockComplete.MarkAsVisited(); // Mark as visited
                    }

                    diceRollerMain?.RegisterVisitedBlock(gameObject);
                }
            }
            else
            {
                stayTimer = 0f;
            }

            if (isCheckingSDG && CurrentSDG != null && !CurrentSDG.activeSelf)
            {
                if (PopUpQuiz != null)
                {
                    PopUpQuiz.SetActive(false);
                }
                isCheckingSDG = false;
            }
        }

        void ActivateQuiz()
        {
            if (OpenQuiz != null && !OpenQuiz.activeSelf)
            {
                OpenQuiz.SetActive(true);
            }

            if (PopUpQuiz != null && !PopUpQuiz.activeSelf)
            {
                PopUpQuiz.SetActive(true);
            }

            if (CurrentSDG != null && PopUpQuiz.activeSelf)
            {
                isCheckingSDG = true;
            }

            isOn = false;
        }
    }
}
