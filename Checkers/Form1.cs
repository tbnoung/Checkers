﻿using Domain;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Checkers
{
    public partial class Form1 : Form, UIListener
    {
        CheckersGame controller;
        Thread thread;

        public Form1()
        {
            InitializeComponent();

            controller = CheckersGame.getInstance();
            controller.addToUIListeners(this);
            controller.Reset();
            gamePanel2.Paint += GamePanel2_Paint;
            gamePanel2.AllowDrop = true;
            richTextBox1.Enabled = false;

            thread = new Thread(GameLoop);
            thread.Start();
        }

        private void GameLoop()
        {
            int TARGET_FPS = 60;
            int waitBetweenFrames = 1000 / TARGET_FPS;


            Stopwatch swFpsCount = new Stopwatch();
            Stopwatch sw = new Stopwatch();


            swFpsCount.Start();
            int framesCount = 0;


            sw.Start();

            while (true)
            {
                
                this.Invoke(new MethodInvoker(delegate { gamePanel2.Refresh(); }));

                if(swFpsCount.ElapsedMilliseconds < 1000)
                {
                    framesCount++;
                }
                else
                {
                    this.Invoke(new MethodInvoker(delegate { label1.Text = framesCount.ToString() + " fps"; })); 
                    framesCount = 1;
                    swFpsCount.Restart();
                }

                int elapsedMS = (int)sw.ElapsedMilliseconds;
                if(elapsedMS < waitBetweenFrames)
                {
                    Thread.Sleep(waitBetweenFrames - elapsedMS);
                }

                sw.Restart();
            }
        }

        private void GamePanel2_Paint(object sender, PaintEventArgs e)
        {
            gamePanel2.repaint(e);
        }        

        private void gamePanel2_MouseMove(object sender, MouseEventArgs e)
        {
            controller.MousePosition = e.Location;
        }

        public void UpdateLog(string log)
        {
            if (richTextBox1.Text.Length > 0)
                richTextBox1.Text += "\r\n";

            richTextBox1.Text += log;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        public void UpdateWinner(string winner)
        {
            if (richTextBox1.Text.Length > 0)
                richTextBox1.Text += "\r\n";

            richTextBox1.Text += "VICTOIRE du joueur : " + winner + "." + "Allez dans le menu 'Dames' > 'Nouvelle partie' pour jouer à nouveau";
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        public void UpdateCurrentPlayer(string currentPlayer)
        {
            lblCurrentPlayer.Text = "Au tour du joueur : " + currentPlayer;
        }

        private void nouvellePartieToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            controller.Reset();
            gamePanel2.Refresh();
            richTextBox1.Clear();
        }     

        private void gamePanel2_MouseDown(object sender, MouseEventArgs e)
        {
            controller.MouseDown();
            gamePanel2.Refresh();
        }

        private void gamePanel2_MouseUp(object sender, MouseEventArgs e)
        {
            controller.MouseUp();
            gamePanel2.Refresh();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread.Abort();
        }
    }
}
