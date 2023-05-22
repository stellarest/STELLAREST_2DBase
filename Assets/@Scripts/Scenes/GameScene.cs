using UnityEngine;

namespace STELLAREST_2D
{
    public class GameScene : MonoBehaviour
    {
        [SerializeField] private GameObject _snakePrefab;
        private GameObject _snake;
        [SerializeField] private GameObject _slimePrefab;
        private GameObject _slime;
        [SerializeField] private GameObject _goblinPrefab;
        private GameObject _goblin;
        [SerializeField] private GameObject _joystickPrefab;
        private GameObject _joystick;

        private void Start()
        {
            _snake = UnityEngine.Object.Instantiate(_snakePrefab);
            _snake.name = _snakePrefab.name;

            _slime = UnityEngine.Object.Instantiate(_slimePrefab);
            _slime.name = _slimePrefab.name;

            _goblin = UnityEngine.Object.Instantiate(_goblinPrefab);
            _goblin.name = _goblinPrefab.name;

            _joystick = UnityEngine.Object.Instantiate(_joystickPrefab);
            //_joystick.name = _joystickPrefab.name;
            _joystick.name = "@UI_Joystick";

            GameObject go = new GameObject() { name = "@Monsters"};
            _snake.transform.parent = go.transform;
            _slime.transform.parent = go.transform;
            _goblin.transform.parent = go.transform;

            var playerController = _slime.AddComponent<PlayerController>(); // Add Component
            _joystick.GetComponent<UI_Joystick>().PlayerController = playerController;

            Camera.main.GetComponent<CameraController>().Target = _slime;
        }
    }
}