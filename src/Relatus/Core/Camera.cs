using Microsoft.Xna.Framework;
using Relatus.Maths;
using Relatus.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Core
{
    public class Camera
    {
        public string Name { get; private set; }
        public Matrix Transform { get; private set; }
        public Matrix World { get; private set; }
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }
        public Matrix WorldViewProjection { get; private set; }
        public Vector3 TopLeft { get; private set; }
        public RectangleF Bounds { get; private set; }
        public Vector2 Center { get => new Vector2(Bounds.X + Bounds.Width * 0.5f, Bounds.Y + Bounds.Height * 0.5f); }
        public float SmoothTrackingSpeed { get; set; }
        public float Zoom { get; private set; }
        public float Rotation { get; private set; }

        private Vector3 cameraPosition;
        private Vector3 cameraTarget;
        private Vector3 cameraCenter; // Not actually the center, really just half the width / height

        private Vector3 trackingPosition;

        private float zoomOffset;

        private bool movementRestricted;
        private float minX;
        private float minY;
        private float maxX;
        private float maxY;

        private bool tracking;
        private bool smoothTracking;

        private bool shaking;
        private bool finishingShake;
        private float shakeRoughness;
        private float shakeDistance;
        private Timer shakeTimer;
        private Vector3 shakePosition;
        private Vector2 originalPosition;

        public Camera(string name) : this(0, 0, name)
        {

        }

        public Camera(float x, float y, string name)
        {
            Name = name;
            TopLeft = new Vector3(x, y, 0);
            trackingPosition = new Vector3(0, 0, 0);
            SmoothTrackingSpeed = 1;

            Initialize();
        }

        public void Reset()
        {
            Initialize();
        }

        public void SetRotation(float rotation)
        {
            if (Rotation == rotation)
                return;

            Rotation = rotation;

            Initialize();
        }

        public void SetZoomOffset(float offset)
        {
            if (zoomOffset == offset)
                return;

            zoomOffset = offset;

            Initialize();
        }

        public void SetTopLeft(Vector2 position)
        {
            SetTopLeft(position.X, position.Y);
        }

        public void SetTopLeft(Vector3 position)
        {
            SetTopLeft(position.X, position.Y);
        }

        public void SetTopLeft(float x, float y)
        {
            if (TopLeft.X == x && TopLeft.Y == y)
                return;

            TopLeft = new Vector3(x, y, TopLeft.Z);

            Initialize();
        }

        public void Track(Vector2 position)
        {
            Track(position.X, position.Y);
        }

        public void Track(float x, float y)
        {
            if (AlreadyTrackingPosition(x, y))
                return;

            tracking = true;
            SetupTracking(x, y);
        }

        public void SmoothTrack(Vector2 position)
        {
            SmoothTrack(position.X, position.Y);
        }

        public void SmoothTrack(float x, float y)
        {
            if (AlreadyTrackingPosition(x, y))
                return;

            smoothTracking = true;
            SetupTracking(x, y);
        }

        public void SetMovementRestriction(float minX, float minY, float maxX, float maxY)
        {
            movementRestricted = true;
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }

        public void Shake(float roughness, float distance, int duration)
        {
            if (shaking)
                return;

            shaking = true;
            finishingShake = false;
            shakeRoughness = roughness;
            shakeDistance = distance;
            shakeTimer = new Timer(duration);
            shakeTimer.Start();
            originalPosition = new Vector2(TopLeft.X, TopLeft.Y);
            shakePosition = TopLeft;
        }

        private void Initialize()
        {
            UpdatePositions();
            UpdateMatrices();
        }

        private void UpdatePositions()
        {
            Zoom = WindowManager.Scale;

            if (WindowManager.WideScreenSupported)
            {
                Bounds = new RectangleF
                (
                    (int)TopLeft.X - WindowManager.PillarBox,
                    (int)TopLeft.Y - WindowManager.LetterBox,
                    WindowManager.PixelWidth + (int)Math.Ceiling(WindowManager.PillarBox * 2),
                    WindowManager.PixelHeight + (int)Math.Ceiling(WindowManager.LetterBox * 2)
                );
                cameraCenter = new Vector3(Bounds.Width / 2 - WindowManager.PillarBox, Bounds.Height / 2 - WindowManager.LetterBox, 0);
            }
            else
            {
                Bounds = new RectangleF
                (
                    (int)TopLeft.X,
                    (int)TopLeft.Y,
                    WindowManager.PixelWidth,
                    WindowManager.PixelHeight
                );
                cameraCenter = new Vector3(Bounds.Width / 2, Bounds.Height / 2, 0);
            }

            cameraPosition = new Vector3(Center.X, -Center.Y, 1);
            cameraTarget = new Vector3(cameraPosition.X, cameraPosition.Y, 0);
        }

        private void UpdateMatrices()
        {
            Transform =
                // M = R * T * S
                // Translate the transform matrix to the inverse of the camera's center.
                Matrix.CreateTranslation(-cameraCenter) *
                // Rotate the camera relative to the center of the camera bounds.
                Matrix.CreateRotationZ(Rotation) *
                // Translate the transform matrix to the transform matrix to the inverse of the camera's top left.
                Matrix.CreateTranslation(-TopLeft) *

                // Scale the transform matrix by the camera's zoom.
                Matrix.CreateScale(Zoom + zoomOffset) *

                // Anchor the transform matrix to the center of the screen instead of the top left.
                Matrix.CreateTranslation(new Vector3(WindowManager.WindowWidth / 2, WindowManager.WindowHeight / 2, 0)) *
                Matrix.Identity;

            World =
                // To be honest, I am not sure why this works.
                // Essentially it rotates the World matrix exactly how the Transform matrix works.
                Matrix.CreateTranslation(-cameraCenter) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(cameraCenter) *

                // In a right-handed coordinate system, the y-axis points upwards. By default, positioning a polygon follows this logic.
                // However, when drawing a sprite using SpriteBatch the y-axis points downwards.
                // Since SpriteBatch is predominately used, the World matrix is rotated so the y-axis also points downwards.
                Matrix.CreateRotationX(MathHelper.Pi) *
                Matrix.Identity;

            View =
                Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);

            Projection =
                Matrix.CreateOrthographic(WindowManager.WindowWidth / (Zoom + zoomOffset), WindowManager.WindowHeight / (Zoom + zoomOffset), 0, 64);

            WorldViewProjection = World * View * Projection;
        }

        private void SetupTracking(float x, float y)
        {
            if (WindowManager.WideScreenSupported)
            {
                trackingPosition = new Vector3(x - Bounds.Width / 2 + WindowManager.PillarBox, y - Bounds.Height / 2 + WindowManager.LetterBox, trackingPosition.Z);
            }
            else
            {
                trackingPosition = new Vector3(x - Bounds.Width / 2, y - Bounds.Height / 2, trackingPosition.Z);
            }
        }

        private bool AlreadyTrackingPosition(float x, float y)
        {
            if (WindowManager.WideScreenSupported)
            {
                if (trackingPosition.X == x - Bounds.Width / 2 + WindowManager.PillarBox && trackingPosition.Y == y - Bounds.Height / 2 + WindowManager.LetterBox)
                {
                    return true;
                }
            }
            else
            {
                if (trackingPosition.X == x - Bounds.Width / 2 && trackingPosition.Y == y - Bounds.Height / 2)
                {
                    return true;
                }
            }

            return false;
        }

        private void NormalCollision()
        {
            if (Bounds.Width > maxX - minX)
            {
                float restrictedRange = maxX - minX;
                SetTopLeft(WindowManager.PillarBox - (Bounds.Width - restrictedRange) / 2, TopLeft.Y);
            }
            else
            {
                if (TopLeft.X < minX)
                {
                    SetTopLeft(minX, TopLeft.Y);
                }

                if (TopLeft.X + Bounds.Width > maxX)
                {
                    SetTopLeft(maxX - Bounds.Width, TopLeft.Y);
                }
            }

            if (Bounds.Height > maxY - minY)
            {
                float restrictedRange = maxY - minY;
                SetTopLeft(TopLeft.X, WindowManager.LetterBox - (Bounds.Height - restrictedRange) / 2);
            }
            else
            {
                if (TopLeft.Y < minY)
                {
                    SetTopLeft(TopLeft.X, minY);

                }
                if (TopLeft.Y + Bounds.Height > maxY)
                {
                    SetTopLeft(TopLeft.X, maxY - Bounds.Height);
                }
            }
        }

        private void WidescreenCollision()
        {
            if (Bounds.Width > maxX - minX)
            {
                float restrictedRange = maxX - minX;
                SetTopLeft(WindowManager.PillarBox - (Bounds.Width - restrictedRange) / 2, TopLeft.Y);
            }
            else
            {
                if (TopLeft.X - WindowManager.PillarBox < minX)
                {
                    SetTopLeft(minX + WindowManager.PillarBox, TopLeft.Y);
                }

                if (TopLeft.X + Bounds.Width - WindowManager.PillarBox > maxX)
                {
                    SetTopLeft(maxX - Bounds.Width + WindowManager.PillarBox, TopLeft.Y);
                }
            }

            if (Bounds.Height > maxY - minY)
            {
                float restrictedRange = maxY - minY;
                SetTopLeft(TopLeft.X, WindowManager.LetterBox - (Bounds.Height - restrictedRange) / 2);
            }
            else
            {
                if (TopLeft.Y - WindowManager.LetterBox < minY)
                {
                    SetTopLeft(TopLeft.X, minY + WindowManager.LetterBox);

                }
                if (TopLeft.Y + Bounds.Height - WindowManager.LetterBox > maxY)
                {
                    SetTopLeft(TopLeft.X, maxY - Bounds.Height + WindowManager.LetterBox);
                }
            }
        }

        private void StayWithinBounds()
        {
            if (!movementRestricted)
                return;

            if (WindowManager.WideScreenSupported)
            {
                WidescreenCollision();
            }
            else
            {
                NormalCollision();
            }
        }

        private void UpdateTracking()
        {
            if (!tracking)
                return;

            SetTopLeft(trackingPosition);
            tracking = false;
        }

        private void UpdateSmoothTracking()
        {
            if (!smoothTracking)
                return;

            SetTopLeft(Vector3.Lerp(TopLeft, trackingPosition, SmoothTrackingSpeed * Engine.DeltaTime));

            Vector3 difference = trackingPosition - TopLeft;
            difference = new Vector3(Math.Abs(difference.X), Math.Abs(difference.Y), Math.Abs(difference.Z));
            float min = 0.5f;
            if (difference.X < min && difference.Y < min && difference.Z < min)
            {
                SetTopLeft(trackingPosition);
                smoothTracking = false;
            }
        }

        private void UpdateShaking()
        {
            if (!shaking)
                return;

            if (!finishingShake)
            {
                Vector3 difference = shakePosition - TopLeft;
                difference = new Vector3(Math.Abs(difference.X), Math.Abs(difference.Y), Math.Abs(difference.Z));
                float min = 0.5f;
                if (difference.X < min && difference.Y < min && difference.Z < min)
                {
                    //float xOffset = (float)RandomHelper.Range(-shakeDistance, shakeDistance);
                    //float yOffset = (float)RandomHelper.Range(-shakeDistance, shakeDistance);
                    float xOffset = (float)Maths.MoreRandom.Gaussian(0, shakeDistance * 0.75);
                    float yOffset = (float)Maths.MoreRandom.Gaussian(0, shakeDistance * 0.75);
                    shakePosition = TopLeft + new Vector3(xOffset, yOffset, 0);
                }

                SetTopLeft(Vector3.Lerp(TopLeft, shakePosition, shakeRoughness * Engine.DeltaTime));

                shakeTimer.Update();

                if (shakeTimer.Done)
                {
                    finishingShake = true;
                }
            }
            else
            {
                SetTopLeft(Vector3.Lerp(TopLeft, new Vector3(originalPosition.X, originalPosition.Y, 0), shakeRoughness * 0.25f * Engine.DeltaTime));

                Vector3 difference = trackingPosition - TopLeft;
                difference = new Vector3(Math.Abs(difference.X), Math.Abs(difference.Y), Math.Abs(difference.Z));
                float min = 0.5f;
                if (difference.X < min && difference.Y < min && difference.Z < min)
                {
                    SetTopLeft(originalPosition);
                    shaking = false;
                }
            }
        }

        public virtual void Update()
        {
            UpdateTracking();
            UpdateSmoothTracking();
            //StayWithinBounds();
            UpdateShaking();
        }
    }
}
