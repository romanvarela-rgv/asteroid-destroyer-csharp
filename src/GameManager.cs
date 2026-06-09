using EngineGDI;
using System;
using System.Xml.Schema;

namespace EngineGDI
{
    public class GameManager
    {
        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }

        private int score;
        private bool isGameOver;

        public int Score { get => score; set => score = value; }
        public bool IsGameOver { get => isGameOver; set => isGameOver = value; }

        private GameManager()
        {
            score = 0;
            isGameOver = false;
        }

        public void Update()
        {
            // Cambios de estado (Menu, juego, Derrota )

            if (isGameOver)
            {
                Console.WriteLine("Game Over! Final Score: " + score);
            }
        }

        public void ResetGame()
        {
            score = 0;
            isGameOver = false;
        }
    }
}

 

    
