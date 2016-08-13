using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;

namespace proto
{
    public class GameLayer : CCLayerColor
    {

        CCSprite ballSpriteGreen;
        CCSprite ballSpritePull;
        CCSprite ballSpriteTarget;
        int TouchID;
        List<CCPoint> movePath;
        bool moveFollow;
        int score;
        CCLabel scoreLabel;
        float topLine;
        float bottomLine;
        bool isTop;
        Random rnd;

        public GameLayer() : base(CCColor4B.Black)
        {
            TouchID = -1;
            topLine = 900;
            bottomLine = 100;
            isTop = true;
            rnd = new Random();

            ballSpriteTarget = new CCSprite("blueball");
            ballSpriteTarget.PositionX = 320;
            ballSpriteTarget.PositionY = topLine;
            ballSpriteTarget.Scale = 5.0f;
            AddChild(ballSpriteTarget);

            ballSpriteGreen = new CCSprite("ball");
            ballSpriteGreen.PositionX = 320;
            ballSpriteGreen.PositionY = topLine;
            ballSpriteGreen.Scale = 5.0f;
            AddChild(ballSpriteGreen);

            ballSpritePull = new CCSprite("pullball");
            ballSpritePull.PositionX = 320;
            ballSpritePull.PositionY = topLine;
            ballSpritePull.Scale = 5.0f;
            AddChild(ballSpritePull);


            scoreLabel = new CCLabel("Score: 0", "Arial", 52, CCLabelFormat.SystemFont);
            scoreLabel.PositionX = 50;
            scoreLabel.PositionY = 1000;
            scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(scoreLabel);

            movePath = new List<CCPoint>();
            moveFollow = false;

            setToPoint();
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            touchListener.OnTouchesBegan = OnTouchesStart;
            touchListener.OnTouchesMoved = OnTouchesMoved;
            AddEventListener(touchListener, this);

            Schedule(RunGameLogic);

        }

        void OnTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0 && TouchID > -1)
            {
                for (int i = 0; i < touches.Count; i++)
                {
                    if (touches[i].Id == TouchID)
                    {
                        movePath.Add(touches[i].Location);
                        ballSpritePull.PositionX = touches[i].Location.X;
                        ballSpritePull.PositionY = touches[i].Location.Y;
                    }
                }
            }

        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                // Perform touch handling here
            }
            if (TouchID != -1)
            {
                moveFollow = true;
                TouchID = -1;
            }
        }


        void OnTouchesStart(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                for (int i = 0; i < touches.Count; i++)
                {
                    if (TouchID == -1)
                    {
                        bool doesBallOverlapPaddle = ballSpritePull.BoundingBoxTransformedToParent.ContainsPoint(
                        touches[i].Location);
                        if (doesBallOverlapPaddle)
                        {
                            TouchID = touches[i].Id;
                        }
                    }
                }
            }
        }

        void RunGameLogic(float frameTimeInSeconds)
        {
            if (moveFollow)
            {
                if (movePath.Count > 0)
                {
                    CCPoint pTo = movePath[0];
                    movePath.RemoveAt(0);
                    ballSpriteGreen.PositionX = pTo.X;
                    ballSpriteGreen.PositionY = pTo.Y;
                    if (ballSpriteGreen.BoundingBoxTransformedToParent.IntersectsRect(ballSpriteTarget.BoundingBoxTransformedToParent))
                    {
                        clearGame();
                        score += 10;
                        scoreLabel.Text = "Score: " + score;
                        setToPoint();
                    }
                }
                else
                {
                    clearGame();
                }
            }
        }

        private void clearGame()
        {
            moveFollow = false;
            movePath.Clear();
            ballSpritePull.PositionX = ballSpriteGreen.PositionX;
            ballSpritePull.PositionY = ballSpriteGreen.PositionY;
        }

        private void setToPoint()
        {
            float lr = rnd.Next(100, 600);

            ballSpriteTarget.PositionX = lr;
            if (isTop)
            {
                ballSpriteTarget.PositionY = bottomLine;
                isTop = false;
            }
            else
            {
                ballSpriteTarget.PositionY = topLine;
                isTop = true;
            }
        }
    }
}

