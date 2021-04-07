using Core;
using Utility;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

namespace Managers
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private Ball ballPrefab;
        [SerializeField] private Shape circleShapePrefab;
        [SerializeField] private Shape squareShapePrefab;
        [SerializeField] private Shape pentagonShapePrefab;
        [SerializeField] private Image nextBallImage;

        private UIManager _uiManager;
        private ScoreManager _scoreManager;

        private Shape _circleShape;
        private Shape _squareShape;
        private Shape _pentagonShape;
        private Shape _currentControlledShape;

        private Ball[] _ballsToSpawn;
        private int _currentBallIndex, _nextBallIndex;
        private int _amountOfBallsOnLevel;

        private ColorPalette _currentColorPalette;
        private Color[] _shapeColors;

        [SerializeField] private float dropInterval;

        private static int _currentLevel;

        private bool _isGameOver;

        private Vector2 _touchPos;

        private Camera _mainCamera;

        private void OnEnable()
        {
            Ball.HitEvent += HitHandler;
            Ball.MissEvent += MissHandler;
        }

        private void OnDisable()
        {
            Ball.HitEvent -= HitHandler;
            Ball.MissEvent -= MissHandler;
        }

        // Start is called before the first frame update
        private void Start()
        {
            string savedColorPalette = PlayerPrefs.GetString("ColorPalette");
            _currentColorPalette = savedColorPalette != "" ?
                Resources.Load<ColorPalette>("ColorPalettes/" + savedColorPalette) : Resources.Load<ColorPalette>("ColorPalettes/Classic");

            _uiManager = FindObjectOfType<UIManager>();
            _scoreManager = FindObjectOfType<ScoreManager>();

            _scoreManager.SetHighScore(PlayerPrefs.GetInt("highscore"));
            _uiManager.SetHighScoreUI(_scoreManager.GetHighScore());

            _mainCamera = Camera.main;

            CreateBalls();
            CreateShapes();
            CreateShapesColors();

            ChangeGameColorPalette();
        }

        // Update is called once per frame
        private void Update()
        {
            if (_isGameOver) return;

            if (Input.touchCount <= 0) return;

            if (IsPointerOverUIObject()) return;

            Touch touch = Input.GetTouch(0);

            if (_mainCamera.ScreenToWorldPoint(touch.position).y < _mainCamera.orthographicSize * 0.5f)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:

                        _touchPos = _mainCamera.ScreenToWorldPoint(touch.position);
                        if (_touchPos.x > 0)
                            _currentControlledShape.RotateShapeRight(0.1f);
                        else if (_touchPos.x < 0)
                            _currentControlledShape.RotateShapeLeft(0.1f);

                        break;

                    case TouchPhase.Moved:

                        break;

                    case TouchPhase.Ended:

                        break;
                }
            }
        }

        private static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        private void CreateBalls()
        {
            _ballsToSpawn = new Ball[5];

            for (int i = 0; i < _ballsToSpawn.Length; i++)
                _ballsToSpawn[i] = Instantiate(ballPrefab, new Vector3(0, 7f, 0), Quaternion.identity);
        }

        private void CreateShapes()
        {
            _circleShape = Instantiate(circleShapePrefab, new Vector3(0, -200f, 0), Quaternion.identity);
            _squareShape = Instantiate(squareShapePrefab, new Vector3(0, -200f, 0), Quaternion.identity);
            _pentagonShape = Instantiate(pentagonShapePrefab, new Vector3(0, -200f, 0), Quaternion.identity);
        }

        private void CreateShapesColors()
        {
            _shapeColors = new Color[5];
            _shapeColors[0] = _currentColorPalette.shapeColor1;
            _shapeColors[1] = _currentColorPalette.shapeColor2;
            _shapeColors[2] = _currentColorPalette.shapeColor3;
            _shapeColors[3] = _currentColorPalette.shapeColor4;
            _shapeColors[4] = _currentColorPalette.shapeColor5;
        }

        private void HitHandler()
        {
            _scoreManager.AddScore(1);
            _uiManager.SetScoreUI(_scoreManager.GetScore());
            SoundManager.Instance.PlayHitSound();
            IncreaseBallDropSpeed();
        }

        private void MissHandler()
        {
            _isGameOver = true;
            SoundManager.Instance.PlayGameOverSound();
            _uiManager.OpenGameOverMenu();
            PauseGame();
            if (_scoreManager.IsNewHighScore())
            {
                PlayerPrefs.SetInt("highscore", _scoreManager.GetHighScore());
                _uiManager.SetHighScoreUI(_scoreManager.GetScore());
            }
            _uiManager.SetGameOverMenuHighScoreUI(_scoreManager.GetScore());
        }

        public void StartLevel(int level)
        {
            _currentLevel = level;

            _scoreManager.SetScore(0);
            _uiManager.SetScoreUI(_scoreManager.GetScore());

            SetAmountOfBallsOnLevel(level);

            ResetBalls();

            SetShapeToLevel(level);

            _nextBallIndex = Random.Range(0, _amountOfBallsOnLevel);
            ChangeNextBallColor(_shapeColors[_nextBallIndex]);

            StopCoroutine("SpawnBall");

            _isGameOver = false;

            ResumeGame();

            StartCoroutine("SpawnBall");
        }

        private void SetAmountOfBallsOnLevel(int level)
        {
            switch (level)
            {
                case 1:
                    _amountOfBallsOnLevel = 3;
                    break;
                case 2:
                    _amountOfBallsOnLevel = 4;
                    break;
                case 3:
                    _amountOfBallsOnLevel = 5;
                    break;
            }
        }

        private void ResetBalls()
        {
            Vector3 ballDefaultPoss = new Vector3(0, 7f, 0);

            foreach (Ball ball in _ballsToSpawn)
            {
                ball.GetComponent<Rigidbody2D>().gravityScale = 0.5f;
                ball.DisableMovement();
                ball.MoveBall(ballDefaultPoss);
            }
        }

        private void IncreaseBallDropSpeed()
        {
            foreach (Ball ball in _ballsToSpawn)
            {
                ball.GetComponent<Rigidbody2D>().gravityScale += 0.01f;
            }
        }

        private void SetShapeToLevel(int level)
        {
            if (_currentControlledShape)
                _currentControlledShape.MoveShapeTo(new Vector3(0f, -200f, 0f));

            switch (level)
            {
                case 1:
                    _currentControlledShape = _circleShape;
                    break;
                case 2:
                    _currentControlledShape = _squareShape;
                    break;
                case 3:
                    _currentControlledShape = _pentagonShape;
                    break;
            }

            _currentControlledShape.MoveShapeTo(new Vector3(0, -2f, 0));
        }

        private IEnumerator SpawnBall()
        {
            while (!_isGameOver)
            {
                _currentBallIndex = _nextBallIndex;
                _ballsToSpawn[_currentBallIndex].EnableMovement();
                ChangeNextBallColor(_shapeColors[_currentBallIndex]);

                _nextBallIndex = Random.Range(0, _amountOfBallsOnLevel);

                yield return new WaitForSeconds(dropInterval);
            }
        }

        public void PauseGame() => Time.timeScale = 0f;

        public void ResumeGame() => Time.timeScale = 1f;

        public void RestartGame()
        {
            ResetBalls();
            StartLevel(_currentLevel);
        }

        #region Color
        public void UseColorPalette(ColorPalette palette)
        {
            if (_currentColorPalette != palette)
            {
                PlayerPrefs.SetString("ColorPalette", palette.name);
                _currentColorPalette = palette;

                if (_shapeColors != null)
                    ChangeGameColorPalette();
            }
        }

        // Change the color of game
        private void ChangeGameColorPalette()
        {
            ResetColors();

            ChangeBallsColors();

            ChangeNextBallColor(_shapeColors[_currentBallIndex]);

            ChangeShapesColors();

            _mainCamera.backgroundColor = _currentColorPalette.backgroundColor;
        }

        private void ResetColors()
        {
            _shapeColors[0] = _currentColorPalette.shapeColor1;
            _shapeColors[1] = _currentColorPalette.shapeColor2;
            _shapeColors[2] = _currentColorPalette.shapeColor3;
            _shapeColors[3] = _currentColorPalette.shapeColor4;
            _shapeColors[4] = _currentColorPalette.shapeColor5;
        }

        private void ChangeBallsColors()
        {
            for (int i = 0; i < _ballsToSpawn.Length; i++)
            {
                _ballsToSpawn[i].SetBallColor(_shapeColors[i]);
            }
        }

        private void ChangeShapesColors()
        {
            ChangeCircleColor();
            ChangeSquareColor();
            ChangePentagonColor();
        }

        private void ChangeCircleColor()
        {
            _circleShape.transform.GetChild(0).GetComponent<SpriteRenderer>().color = _shapeColors[0];
            _circleShape.transform.GetChild(1).GetComponent<SpriteRenderer>().color = _shapeColors[1];
            _circleShape.transform.GetChild(2).GetComponent<SpriteRenderer>().color = _shapeColors[2];
        }

        private void ChangeSquareColor()
        {
            _squareShape.transform.GetChild(0).GetComponent<SpriteRenderer>().color = _shapeColors[0];
            _squareShape.transform.GetChild(1).GetComponent<SpriteRenderer>().color = _shapeColors[1];
            _squareShape.transform.GetChild(2).GetComponent<SpriteRenderer>().color = _shapeColors[2];
            _squareShape.transform.GetChild(3).GetComponent<SpriteRenderer>().color = _shapeColors[3];
        }

        private void ChangePentagonColor()
        {
            _pentagonShape.transform.GetChild(0).GetComponent<SpriteRenderer>().color = _shapeColors[0];
            _pentagonShape.transform.GetChild(1).GetComponent<SpriteRenderer>().color = _shapeColors[1];
            _pentagonShape.transform.GetChild(2).GetComponent<SpriteRenderer>().color = _shapeColors[2];
            _pentagonShape.transform.GetChild(3).GetComponent<SpriteRenderer>().color = _shapeColors[3];
            _pentagonShape.transform.GetChild(4).GetComponent<SpriteRenderer>().color = _shapeColors[4];
        }

        private void ChangeNextBallColor(Color color) => nextBallImage.color = color;

        #endregion
    }
}