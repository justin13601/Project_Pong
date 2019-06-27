using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_PongTests
{
    [TestClass()]
    public class PongGameTests
    {
        //Tests for when Player 1 Scores
        [TestMethod()]
        public void CheckScore_ForPlayer1ScoreChanged_ReturnTrue()
        {
            //Arrange
            Rectangle Ball = new Rectangle(778, 115, 16, 16);
            Project_Pong.Form1.PongGame Test = new Project_Pong.Form1.PongGame(Ball);

            //Act
            bool ScoreChanged = Test.CheckScore(1, 1);

            //Assert
            Assert.IsTrue(ScoreChanged);
        }

        //Tests for when Player 2 Scores
        [TestMethod()]
        public void CheckScore_ForPlayer2ScoreChanged_ReturnTrue()
        {
            //Arrange
            Rectangle Ball = new Rectangle(0, 115, 16, 16);
            Project_Pong.Form1.PongGame Test = new Project_Pong.Form1.PongGame(Ball);

            //Act
            bool ScoreChanged = Test.CheckScore(1, 1);

            //Assert
            Assert.IsTrue(ScoreChanged);
        }

        //Tests for when Player 1 Wins
        [TestMethod()]
        public void CheckWin_ForPlayer1Wins_ReturnTrue()
        {
            //Arrange
            Project_Pong.Form1.PongGame Test = new Project_Pong.Form1.PongGame();

            //Act
            bool Player1Won = Test.CheckWin(11, 8);

            //Assert
            Assert.IsTrue(Player1Won);
        }

        //Test for when Player 2 Wins
        [TestMethod()]
        public void CheckWin_ForPlayer2Wins_ReturnTrue()
        {
            //Arrange
            Project_Pong.Form1.PongGame Test = new Project_Pong.Form1.PongGame();

            //Act
            bool Player2Won = Test.CheckWin(8, 11);

            //Assert
            Assert.IsTrue(Player2Won);
        }

        //Test for when the game board resets after Player 1 Scores
        [TestMethod()]
        public void Restart_ForPlayer1Score_ChecksXSpeed()
        {
            //Arrange
            Project_Pong.Form1.PongGame Test = new Project_Pong.Form1.PongGame();

            //Act
            Test.Restart(1);
            int XSpeed = Test.Xspeed;

            //Assert
            Assert.AreEqual(XSpeed, 1);
        }

        //Test for when the game board resets after Player 2 Scores
        [TestMethod()]
        public void Restart_ForPlayer2Score_ChecksXSpeed()
        {
            //Arrange
            Project_Pong.Form1.PongGame Test = new Project_Pong.Form1.PongGame();

            //Act
            Test.Restart(2);
            int XSpeed = Test.Xspeed;

            //Assert
            Assert.AreEqual(XSpeed, -1);
        }

    }
}