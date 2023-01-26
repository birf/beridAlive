using UnityEngine;
using BeriUtils.Core;
namespace RPG.Combat
{

    public enum BLOCKSTATE { NO_BLOCK, BLOCK, COOLDOWN };
    public class BlockScript : MonoBehaviour
    {
        [SerializeField] float cooldownDuration;
        [SerializeField] float blockDuration;

        private Timer blockTimer;
        private Timer cooldownTimer;

        [SerializeField] float parryOpeningTime;

        enum BLOCKTYPE { NO_BLOCK, NORMAL, PARRY }

        [SerializeField] BLOCKTYPE currentBlockType;
        [SerializeField] BLOCKSTATE currentBlockState;

        SpriteRenderer spriteRenderer;

        [SerializeField] Color idleColor;

        PrimaryControls controls;



        void OnEnable()
        {
            controls = new PrimaryControls();
            controls.Enable();
        }
        //void OnDisable()
        //{
        //    DisableCursor();
        //}


        public void Block()
        {
            spriteRenderer.color = Color.yellow;
            currentBlockState = BLOCKSTATE.BLOCK;
            //GetComponent<ActionScheduler>().StartAction(this);
        }

        //public void Cancel()
        //{
        //    this.enabled = false;
        //}

        private void Start()
        {
            blockTimer = new Timer(blockDuration);
            cooldownTimer = new Timer(cooldownDuration);


            blockTimer.OnTimerStart += Block;
            blockTimer.OnTimerEnd += StopBlock;

            cooldownTimer.OnTimerStart += Cooldown;
            cooldownTimer.OnTimerEnd += StopCooldown;

            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Cooldown()
        {
            currentBlockState = BLOCKSTATE.COOLDOWN;
            spriteRenderer.color = Color.blue;

        }

        private void StopCooldown()
        {
            spriteRenderer.color = idleColor;
            blockTimer.SetTimer(blockDuration);
            cooldownTimer.SetTimer(cooldownDuration);
            currentBlockType = BLOCKTYPE.NO_BLOCK;
            currentBlockState = BLOCKSTATE.NO_BLOCK;
        }

        private void StopBlock(bool hasTakenDamage)

        {
            if (hasTakenDamage)
            {
                ReturnToNormalState();
            }
            else
            {
                Cooldown();
            }

        }

        private void ReturnToNormalState()
        {
            spriteRenderer.color = idleColor;
            blockTimer.SetTimer(blockDuration);
            cooldownTimer.SetTimer(cooldownDuration);
            currentBlockType = BLOCKTYPE.NO_BLOCK;
        }

        private void StopBlock()
        {
            if (currentBlockType == BLOCKTYPE.PARRY || currentBlockType == BLOCKTYPE.NORMAL)
            {
                ReturnToNormalState();
            }
            else
            {
                Cooldown();
                return;
            }


        }

        private void DetermineBlockType()
        {
            if (blockTimer.GetRemaingingSeconds() > blockDuration - parryOpeningTime)
            {
                currentBlockType = BLOCKTYPE.PARRY;
            }

            else
            {
                currentBlockType = BLOCKTYPE.NORMAL;
            }


        }

        private void Update()
        {
            switch (currentBlockState)
            {
                case BLOCKSTATE.BLOCK:
                    blockTimer.Tick(Time.deltaTime);
                    break;

                case BLOCKSTATE.COOLDOWN:
                    cooldownTimer.Tick(Time.deltaTime);
                    break;
            }


            if (controls.Battle.Primary.triggered && BattleManager.CurrentBattleManagerState == BattleManager.BattleManagerState.ENEMYTURN)
            {
                Block();
            }
        }

        private void DetermineDamageOutput(BLOCKTYPE blockType)
        {
            switch (blockType)
            {
                case BLOCKTYPE.NO_BLOCK:
                    Debug.Log("Take Full Damage");
                    break;
                case BLOCKTYPE.NORMAL:
                    Debug.Log("Take Half Damage");
                    break;
                case BLOCKTYPE.PARRY:
                    Debug.Log("Take no Damage");
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("enemy_projectile"))
            {
                if (currentBlockState == BLOCKSTATE.BLOCK)
                {
                    DetermineBlockType();
                }
                DetermineDamageOutput(currentBlockType);
                StopCooldown();


            }
        }


    }
}