using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player info")]
    [SerializeField] private float mouseSencitivity = 10f;
    [SerializeField] private float cameraLimitAngle = 70f;

    [Header("UI info")]
    [SerializeField]private CastUiController castUiController;

    private Vector3 velocity = Vector3.zero;
    private Vector3 move = Vector3.zero;
    float xRotation = 0f;
    private Camera playerCamera;
    private CharacterController characterController;
    private GameInput gameInput;
    private Buoyancy buoyancyController;
    [SerializeField]private float currentCastPower = 0f;
    private float factorCastPower = 100f;

    [SerializeField] private FishingRodController fishingrodController = null;
    [SerializeField] private FishingReelController fishingReelController = null;
    [SerializeField] private FloaterController floaterController = null;
    [SerializeField] private CargoController cargoController = null;
    [SerializeField] private HookController hookController = null;

    [SerializeField] private bool invertTemp = false;


    //StateGame
    public enum GameSate { noCollect, isReady, isCast, isWater }
    [SerializeField] private GameSate gameState = GameSate.noCollect;

    private void Awake()
    {
        gameInput = new GameInput();
        gameInput.Game.Enable();
    }

    private void Start()
    {
        playerCamera = Camera.main;
        characterController = GetComponent<CharacterController>();

        fishingrodController.SetCargoRigidbody(cargoController.gameObject.GetComponent<Rigidbody>());
        fishingrodController.SetCargoBuoyancyController(cargoController.gameObject.GetComponent<Buoyancy>());
    }
    private void Update()
    {
        switch (gameState)
        {
            case GameSate.noCollect:
                looking();
                break;
            case GameSate.isReady:
                looking();
                SetFloaterDepth();
                //CastFihingRod();

                if (gameInput.Game.Cast.WasPressedThisFrame())
                {
                    gameState = GameSate.isCast;
                    castUiController.SetCastSliderActive(true);

                    SetIsReadyAll(false);
                }
                break;
            case GameSate.isCast:
                CastFihingRod();
                looking();
                break;
            case GameSate.isWater:
                FishingRodHooking();
                WindingFishingRod();
                looking();
                break;
            default:
                break;
        }
    }

        private void looking()
    {
        Vector2 mouseAxis = gameInput.Game.Looking.ReadValue<Vector2>() * Time.smoothDeltaTime;
        float mouseX = mouseAxis.x * mouseSencitivity;
        float mouseY = mouseAxis.y * mouseSencitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -cameraLimitAngle, cameraLimitAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    private void FishingRodHooking()
    {
        if (gameInput.Game.Hooking.IsPressed())
        {
            Debug.Log("Hooking");
        }
        else if (gameInput.Game.Hooking.WasPerformedThisFrame())
        {

        }
    }

    private void WindingFishingRod()
    {
        if (gameInput.Game.Winding.IsPressed())
        {
            fishingrodController.UpdateWinch(invertTemp);
        }

        if(floaterController != null)
        {
            if (fishingrodController.GetIsReady(floaterController.GetRopeLength()))
            {
                gameState = GameSate.isReady;

                SetIsReadyAll(true);

            }
        }
        else
        {
            if (fishingrodController.GetIsReady(floaterController.GetRopeLength()))
            {
                gameState = GameSate.isReady;

                SetIsReadyAll(true);

            }
        }


    }

    private void SetFloaterDepth()
    {
        if (gameInput.Game.FloaterDepthUp.IsPressed())
        {
            floaterController.UpdateFloaterDepth(true);
            fishingrodController.SetFishingRodFloaterDepth(floaterController.GetRopeLength());
        }
        else if (gameInput.Game.FloaterDepthDown.IsPressed())
        {
            floaterController.UpdateFloaterDepth(false);
            fishingrodController.SetFishingRodFloaterDepth(floaterController.GetRopeLength());
        }
    }

    private void CastFihingRod()
    {
        if (gameInput.Game.Cast.IsPressed())
        {
            currentCastPower += factorCastPower * Time.deltaTime;
            if (currentCastPower > 100f)
                currentCastPower = 100f;
            castUiController.SetCastPowerSlider(currentCastPower);
        }
        if (gameInput.Game.Cast.WasPerformedThisFrame())
        {
            gameState = GameSate.isWater;
            castUiController.SetCastSliderActive(false);
            fishingrodController.CastFishingRod(currentCastPower, transform.forward);
            currentCastPower = 0f;
        }

    }
    private void SetIsReadyAll(bool isReady)
    {
        if (fishingrodController != null)
        {
            fishingrodController.SetIsReadyFishinfRod(isReady);
        }
        if (fishingReelController != null)
        {
            //fishingReelController.SetIsReadyFishinfRod(isReady);
        }
        if (floaterController != null)
        {
            floaterController.SetIsReadyFishinfRod(isReady);
        }
        if (cargoController != null)
        {
            //cargoController.SetIsReadyFishinfRod(isReady);
        }
        if (hookController != null)
        {
            //hookController.SetIsReadyFishinfRod(isReady);
        }
    }

}