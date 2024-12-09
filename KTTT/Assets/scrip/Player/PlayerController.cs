using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("UI info")]
    [SerializeField] private CastUiController castUiController;

    private Vector3 velocity = Vector3.zero;
    private Vector3 move = Vector3.zero;
    float xRotation = 0f;
    private CharacterController characterController;
    private GameInput gameInput;
    private Buoyancy buoyancyController;
    [SerializeField] private float currentCastPower = 0f;
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
        characterController = GetComponent<CharacterController>();

        fishingrodController.SetCargoRigidbody(cargoController.gameObject.GetComponent<Rigidbody>());
        fishingrodController.SetCargoBuoyancyController(cargoController.gameObject.GetComponent<Buoyancy>());
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameSate.noCollect:
                looking(); // Nếu cần, bạn có thể bỏ qua phần này
                break;
            case GameSate.isReady:
                looking(); // Bạn có thể bỏ qua đây nếu không cần điều khiển camera
                SetFloaterDepth();
                if (gameInput.Game.Cast.WasPressedThisFrame())
                {
                    gameState = GameSate.isCast;
                    castUiController.SetCastSliderActive(true);
                    SetIsReadyAll(false);
                }
                break;
            case GameSate.isCast:
                CastFihingRod();
                looking(); // Bỏ qua điều khiển camera nếu không cần
                break;
            case GameSate.isWater:
                FishingRodHooking();
                WindingFishingRod();
                looking(); // Bỏ qua điều khiển camera nếu không cần
                break;
            default:
                break;
        }
    }

    private void looking()
    {
        // Bỏ qua việc điều khiển camera và góc nhìn nếu không cần thiết
    }

    private void FishingRodHooking()
    {
        if (gameInput.Game.Hooking.IsPressed())
        {
            Debug.Log("Hooking");
        }
        else if (gameInput.Game.Hooking.WasPerformedThisFrame())
        {
            // Xử lý hook khi cần
        }
    }

    private void WindingFishingRod()
    {
        // Kiểm tra nếu trong trạng thái isWater và người chơi nhấn phím thu dây
        if (gameState == GameSate.isWater && gameInput.Game.Winding.IsPressed())
        {
            
            fishingrodController.UpdateWinch(invertTemp);
        }

        // Kiểm tra nếu đã thu dây về mức độ đủ sẵn sàng
        if (floaterController != null)
        {
            if (fishingrodController.GetIsReady(floaterController.GetRopeLength()))
            {
                gameState = GameSate.isReady;
                SetIsReadyAll(true); // Đặt tất cả thành sẵn sàng
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

            // Quăng cần câu theo hướng 90 độ so với hướng của nhân vật
            fishingrodController.CastFishingRod(currentCastPower, transform.right);  // Quăng sang phải
            // fishingrodController.CastFishingRod(currentCastPower, -transform.right); // Quăng sang trái
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
