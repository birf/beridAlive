using UnityEngine;
using BeriUtils.Core;

namespace RPG.Combat
{

    public enum BLOCKSTATE { NO_BLOCK, BLOCK, COOLDOWN };
    
    [RequireComponent(typeof(CharacterGameBattleEntity))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BlockScript : MonoBehaviour
    {
        /*
            Script for allowing the player to block and parry incoming attacks. 
        */
        [SerializeField] float cooldownDuration;
        [SerializeField] float blockDuration;

        private Timer blockTimer;
        private Timer cooldownTimer;

        [SerializeField] float parryOpeningTime;
        [SerializeField] BLOCKSTATE currentBlockState;
        [SerializeField] CharacterGameBattleEntity characterBody;

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

        private void Awake()
        {
            blockTimer = new Timer(blockDuration);
            cooldownTimer = new Timer(cooldownDuration);


            blockTimer.OnTimerStart += Block;
            blockTimer.OnTimerEnd += StopBlock;

            cooldownTimer.OnTimerStart += Cooldown;
            cooldownTimer.OnTimerEnd += StopCooldown;

            spriteRenderer = GetComponent<SpriteRenderer>();
            characterBody = GetComponent<CharacterGameBattleEntity>();
        }

        public void Cooldown()
        {
            currentBlockState = BLOCKSTATE.COOLDOWN;
            spriteRenderer.color = Color.blue;

        }

        public void StopCooldown()
        {
            spriteRenderer.color = idleColor;
            blockTimer.SetTimer(blockDuration);
            cooldownTimer.SetTimer(cooldownDuration);
            currentBlockState = BLOCKSTATE.NO_BLOCK;
        }

        public void StopBlock(bool hasTakenDamage)

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
        }

        private void StopBlock()
        {
            if (currentBlockState == BLOCKSTATE.BLOCK)
            {
                ReturnToNormalState();
            }
            else
            {
                Cooldown();
                return;
            }
        }

        public bool DetermineBlockType()
        {
            if (blockTimer.GetRemaingingSeconds() > blockDuration - parryOpeningTime)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        private void Update()
        {
            switch (currentBlockState)
            {
                case BLOCKSTATE.BLOCK:
                    blockTimer.Tick(Time.deltaTime);
                    characterBody.characterBattlePhysics.characterPhysicsState = BattlePhysicsInteraction.CharacterPhysicsState.BLOCKING;
                    break;
                case BLOCKSTATE.COOLDOWN:
                    cooldownTimer.Tick(Time.deltaTime);
                    characterBody.characterBattlePhysics.characterPhysicsState = BattlePhysicsInteraction.CharacterPhysicsState.DEFAULT;
                    break;
            }


            if (controls.Battle.Primary.triggered && BattleManager.CurrentBattleManagerState == BattleManager.BattleManagerState.ENEMYTURN)
            {
                Block();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("in second");
            if (collision.CompareTag("enemy_projectile"))
            {
                if (currentBlockState == BLOCKSTATE.BLOCK)
                {
                    DetermineBlockType();
                }
                StopCooldown();
            }
        }
    }
}