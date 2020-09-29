using HorseSaddle.Wheel.Segments;
using LiruGameHelper.XML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace HorseSaddle.Wheel
{
    public class SpinningWheel<T> where T : Segment
    {
        #region XML Constants
        private const string nameAttributeName = "Name";

        private const string radiusAttributeName = "Radius";

        private const string repeatNodeName = "Repeat";

        private const string amountAttributeName = "Amount";

        private const string textPaddingAttributeName = "TextPadding";

        private const string dragAttributeName = "Drag";

        private const string innerImageAttributeName = "InnerImage";

        private const string indicatorImageAttributeName = "IndicatorImage";

        private const string iconImageAttributeName = "IconImage";

        private const string resourcesFolderName = "Resources";
        #endregion

        #region Dependencies
        private readonly Saddle mainGame = null;
        #endregion

        #region Fields
        private readonly List<T> segments = new List<T>();

        private float rotation = 0;

        private float indicatorRotation = -MathHelper.PiOver2;
        #endregion

        #region Graphical Properties
        /// <summary> The wheel background. </summary>
        public WheelBackground Background { get; }

        /// <summary> The font used for the segment names. </summary>
        public SpriteFont Font { get; set; }

        /// <summary> How many pixels from the edge the text is placed. </summary>
        public float TextPadding { get; set; } = 7.5f;

        /// <summary> The texture used to indicate the current segment. </summary>
        public Texture2D IndicatorImage { get; set; }

        /// <summary> How deep into the wheel the indicator is drawn, in pixels. </summary>
        public float IndicatorDepth { get; set; } = 7;

        /// <summary> The image displayed in the centre of the wheel. </summary>
        public Texture2D InnerImage { get; set; }

        /// <summary> The icon used to represent the wheel. </summary>
        public Texture2D Icon { get; set; }

        /// <summary> The position of the centre of the wheel on the screen. </summary>
        public Vector2 ScreenPosition { get; set; }
        #endregion

        #region Properties
        /// <summary> The name of this wheel. </summary>
        public string Name { get; set; }

        /// <summary> How many segments are on the wheel. </summary>
        public int SegmentCount => segments.Count;

        /// <summary> The radius of the wheel. </summary>
        public ushort Radius
        {
            get => Background.Radius;
            set => Background.Radius = value;
        }

        /// <summary> The current rotation in radians. </summary>
        public float Rotation
        {
            get => rotation;
            set => rotation = MathHelper.WrapAngle(value);
        }

        /// <summary> The radians per second applied to the <see cref="Rotation"/>. </summary>
        public float RotationalSpeed { get; set; }

        /// <summary> The radians per second taken from the <see cref="RotationalSpeed"/>. </summary>
        public float RotationalDrag { get; set; } = MathHelper.TwoPi;

        /// <summary> Is true when the wheel is not spinning, false otherwise. </summary>
        public bool IsStopped => RotationalSpeed == 0;

        /// <summary> The rotation around the wheel in radians where the indicator is located. </summary>
        public float IndicatorRotation
        {
            get => indicatorRotation;
            set => indicatorRotation = MathHelper.WrapAngle(value);
        }

        /// <summary> The index of the current segment that the indicator is over. </summary>
        public int CurrentSegmentIndex
        {
            get
            {
                // Calculate how much of the half circumference a single segment takes.
                float segmentRatio = 2.0f / SegmentCount;

                // Normalise the rotation so that it ranges from -1 to 1. Where -0.5 is pointing upwards and 0.5 is pointing downwards. Add in the rotation of the indicator as an offset.
                float normalisedRotation = MathHelper.WrapAngle(Rotation - IndicatorRotation) / MathHelper.Pi;

                // Calculate the negative/positive segment. If the segment count is odd, then add half the ratio to the normalised rotation.
                float segment = (normalisedRotation - ((SegmentCount % 2 == 0) ? 0 : (segmentRatio / 2))) / segmentRatio;

                // If the segment is negative, then the final segment is the absolute value of it. Otherwise, the segment is the inverse so the count is subtracted from it.
                segment = segment < 0 ? Math.Abs(segment) : SegmentCount - segment;

                // Technically if the angle is exactly pi or -pi, the segment will be the segment count, so ensure it can only go as high as the last element.
                return (int)Math.Min(SegmentCount - 1, Math.Floor(segment));
            }
        }

        /// <summary> The current segment that the indicator is over. </summary>
        public T CurrentSegment => SegmentCount > 0 ? segments[CurrentSegmentIndex] : null;
        #endregion

        #region Constructors
        public SpinningWheel(Saddle mainGame, WheelBackground background, SpriteFont font, int radius, IReadOnlyList<T> segments)
        {
            // Set dependencies.
            this.mainGame = mainGame ?? throw new ArgumentNullException(nameof(mainGame));
            
            // Create the wheel background.
            Background = background;
            Font = font;

            // If segment names were given, add them to the list.
            AddSegments(segments);

            // Set the radius.
            if (radius < ushort.MinValue || radius > ushort.MaxValue) throw new ArgumentException("Given radius is out of range.");
            Radius = (ushort)radius;
        }
        #endregion

        #region Rotation Functions
        public void RandomiseRotation(Random random) => Rotation = (float)(Math.PI * ((random.NextDouble() * 2) - 1));
        #endregion

        #region Segment Functions
        /// <summary> Adds a new segment to the wheel with the given <paramref name="segment"/>. </summary>
        /// <param name="segment"> The new segment. </param>
        public void AddSegment(T segment)
        {
            // Add the segment name.
            segments.Add(segment);

            // Set the segment count of the background to the same as the amount of segments on the wheel.
            Background.SegmentCount = segments.Count;
        }

        /// <summary> Copies every name from the given <paramref name="segments"/> into new segments. </summary>
        /// <param name="segments"> The list of segments. </param>
        public void AddSegments(IReadOnlyList<T> segments)
        {
            // Don't do anything if the given list was null.
            if (segments == null) return;

            // Add the segment names to the list.
            this.segments.AddRange(segments);

            // Set the segment count of the background to the same as the amount of segments on the wheel.
            Background.SegmentCount = segments.Count;
        }

        public void RemoveCurrentSegment()
        {
            // If there's no segments to remove, do nothing.
            if (SegmentCount == 0) return;

            // Remove the segment.
            segments.RemoveAt(CurrentSegmentIndex);

            // Set the segment count of the background to the same as the amount of segments on the wheel.
            Background.SegmentCount = segments.Count;
        }
        #endregion

        #region Update Functions
        public void Update(GameTime gameTime)
        {
            // Apply drag to the rotational speed, stopping at 0.
            RotationalSpeed = (float)Math.Max(0, RotationalSpeed - (RotationalDrag * gameTime.ElapsedGameTime.TotalSeconds));
                    
            // Apply the rotational speed to the rotation.
            Rotation += (float)(RotationalSpeed * gameTime.ElapsedGameTime.TotalSeconds);
        }
        #endregion

        #region Draw Functions
        public void Draw(SpriteBatch spriteBatch)
        {
            // If there are no segments, do nothing.
            if (segments.Count == 0) return;

            // Draw the wheel background.
            Background.Draw(spriteBatch, ScreenPosition, Rotation);

            // Draw the inner image.
            drawInnerImage(spriteBatch);

            // Draw the segment names.
            drawSegmentNames(spriteBatch);

            // Draw the indicator.
            drawIndicator(spriteBatch);
        }

        private void drawSegmentNames(SpriteBatch spriteBatch)
        {
            // If no font exists, don't draw the names.
            if (Font == null) return;

            // Calculate the ratio between the entire circumference and each segment's size.
            float segmentSize = 1.0f / SegmentCount;

            // Go over each segment name and draw them.
            for (int i = 0; i < Math.Min(SegmentCount, segments.Count); i++)
            {
                // Measure the string with the font and save the size.
                Vector2 stringSize = Font.MeasureString(segments[i].Name);

                // Calculate the angle of the text.
                float textAngle = MathHelper.WrapAngle(rotation + (((segmentSize * i) + (SegmentCount % 2 == 0 ? segmentSize / 2 : 0)) * MathHelper.TwoPi));

                // Caclulate the position of the text based on its size and angle.
                Vector2 textPosition = ScreenPosition + new Vector2((float)Math.Cos(textAngle), (float)Math.Sin(textAngle)) * (Radius - (stringSize.X + TextPadding));

                Color textColour = Color.White;// new Color(Vector3.One - segmentColours[i % segmentColours.Count].ToVector3());

                // Finally, draw the rotated text.
                spriteBatch.DrawString(Font, segments[i].Name, textPosition + new Vector2(2), new Color(0.1f, 0.1f, 0.1f, 0.9f), textAngle, new Vector2(0, stringSize.Y / 2), 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(Font, segments[i].Name, textPosition, textColour, textAngle, new Vector2(0, stringSize.Y / 2), 1, SpriteEffects.None, 0);
            }
        }

        private void drawIndicator(SpriteBatch spriteBatch)
        {
            // If no indicator texture exists, do nothing.
            if (IndicatorImage == null) return;

            // The position of the indicator is essentially on the edge of the wheel based on the indicator rotation.
            Vector2 position = ScreenPosition + (new Vector2((float)Math.Cos(IndicatorRotation), (float)Math.Sin(IndicatorRotation)) * (Radius - IndicatorDepth));

            // Draw the indicator.
            spriteBatch.Draw(IndicatorImage, position, null, Color.White, IndicatorRotation, IndicatorImage.Bounds.Size.ToVector2() * new Vector2(0, 0.5f), 1, SpriteEffects.None, 0);
        }

        private void drawInnerImage(SpriteBatch spriteBatch)
        {
            // If no inner image texture exists, do nothing.
            if (InnerImage == null) return;

            // Draw the inner image directly in the centre.
            spriteBatch.Draw(InnerImage, ScreenPosition, null, Color.White, Rotation, InnerImage.Bounds.Size.ToVector2() / 2, 1, SpriteEffects.None, 0);
        }
        #endregion

        #region Load Functions
        public void ReloadSegmentsFromFile(string fileName)
        {
            // Ensure the filename is valid and exists.
            if (string.IsNullOrWhiteSpace(fileName) || !System.IO.File.Exists(fileName)) throw new System.IO.FileNotFoundException($"The given file path was invalid or could not be found.", fileName);

            // Load as an xml file.
            XmlDocument wheelFile = new XmlDocument();
            wheelFile.Load(fileName);

            // Get the node for this specific wheel.
            XmlNode wheelNode = wheelFile.LastChild.SelectSingleNode($"*[@{nameAttributeName}='{Name}']");

            // Clear the segments.
            segments.Clear();

            // Load each segment.
            foreach (XmlNode segmentNode in wheelNode)
            {
                // If this segment is actually a repeat command, do so.
                if (segmentNode.Name == repeatNodeName)
                    // Repeat for the number of repititons, going over each segment within the repeat command.
                    for (int i = 0; i < segmentNode.ParseAttributeValue(amountAttributeName, int.Parse); i++)
                        foreach (XmlNode repeatSegmentNode in segmentNode)
                            AddSegment((T)Segment.LoadFromXMLNode(repeatSegmentNode, mainGame));
                // Otherwise; just add the segment.
                else AddSegment((T)Segment.LoadFromXMLNode(segmentNode, mainGame));
            }
        }

        public static List<SpinningWheel<T>> LoadWheelsFromXML(GraphicsDevice graphicsDevice, Saddle mainGame, SpriteFont font, string fileName)
        {
            // Ensure the filename is valid and exists.
            if (string.IsNullOrWhiteSpace(fileName) || !System.IO.File.Exists(fileName)) throw new System.IO.FileNotFoundException($"The given file path was invalid or could not be found.", fileName);

            // Load as an xml file.
            XmlDocument wheelFile = new XmlDocument();
            wheelFile.Load(fileName);

            // Get the main wheels node.
            XmlNode wheelsNode = wheelFile.LastChild;

            // Create a new list for the wheels, then go over each child node of the wheels node and load them as wheels.
            List<SpinningWheel<T>> wheels = new List<SpinningWheel<T>>(wheelsNode.ChildNodes.Count);
            foreach (XmlNode wheelNode in wheelsNode)
                wheels.Add(LoadFromXMLNode(graphicsDevice, mainGame, font, wheelNode));

            // Return the created list.
            return wheels;
        }

        public static SpinningWheel<T> LoadFromXMLNode(GraphicsDevice graphicsDevice, Saddle mainGame, SpriteFont font, XmlNode wheelNode)
        {
            // Load the graphical data of the wheel.
            WheelBackground wheelGraphic = WheelBackground.LoadFromXML(graphicsDevice, wheelNode);

            // Parse the radius of the wheel.
            ushort radius = wheelNode.ParseAttributeValue(radiusAttributeName, ushort.Parse);

            // Create a list to hold the segments.
            List<T> segmentNames = new List<T>();

            // Load each segment.
            foreach (XmlNode segmentNode in wheelNode)
            {
                // If this segment is actually a repeat command, do so.
                if (segmentNode.Name == repeatNodeName)
                    // Repeat for the number of repititons, going over each segment within the repeat command.
                    for (int i = 0; i < segmentNode.ParseAttributeValue(amountAttributeName, int.Parse); i++)
                        foreach (XmlNode repeatSegmentNode in segmentNode)
                            segmentNames.Add((T)Segment.LoadFromXMLNode(repeatSegmentNode, mainGame));
                // Otherwise; just add the segment.
                else segmentNames.Add((T)Segment.LoadFromXMLNode(segmentNode, mainGame));
            }

            // Create the loaded wheel.
            SpinningWheel<T> wheel = new SpinningWheel<T>(mainGame, wheelGraphic, font, radius, segmentNames);

            // Parse the post-settings.
            if (wheelNode.Attributes.GetNamedItem(textPaddingAttributeName) != null) wheel.TextPadding = wheelNode.ParseAttributeValue(textPaddingAttributeName, float.Parse);
            if (wheelNode.Attributes.GetNamedItem(dragAttributeName) != null) wheel.RotationalDrag = wheelNode.ParseAttributeValue(dragAttributeName, float.Parse);

            // Set the wheel name.
            wheel.Name = wheelNode.GetAttributeValue(nameAttributeName);

            // Create the resources file path.
            string resourcesFilePath = Path.Combine(Path.GetDirectoryName(mainGame.WheelPresetPath), resourcesFolderName);

            // If an inner image URI was given, load the image.
            if (wheelNode.Attributes.GetNamedItem(innerImageAttributeName) != null)
            {
                // Create the path using the resources path and the name of the image.
                string innerImagePath = Path.Combine(resourcesFilePath, wheelNode.GetAttributeValue(innerImageAttributeName));

                // Load the texture.
                Texture2D innerImage;
                using (FileStream innerImageStream = File.OpenRead(innerImagePath))
                {
                    innerImage = Texture2D.FromStream(graphicsDevice, innerImageStream);
                }

                // Set the inner image of the wheel to the loaded texture.
                wheel.InnerImage = innerImage;
            }

            // If an indicator image URI was given, load it.
            if (wheelNode.Attributes.GetNamedItem(indicatorImageAttributeName) != null)
            {
                // Create the path using the resources path and the name of the image.
                string indicatorImagePath = Path.Combine(resourcesFilePath, wheelNode.GetAttributeValue(indicatorImageAttributeName));

                // Load the texture.
                Texture2D indicatorImage;
                using (FileStream indicatorImageStream = File.OpenRead(indicatorImagePath))
                {
                    indicatorImage = Texture2D.FromStream(graphicsDevice, indicatorImageStream);
                }

                // Set the indicator image of the wheel to the loaded texture.
                wheel.IndicatorImage = indicatorImage;
            }

            // If an icon image URI was given, load it.
            if (wheelNode.Attributes.GetNamedItem(iconImageAttributeName) != null)
            {
                // Create the path using the resources path and the name of the image.
                string iconImagePath = Path.Combine(resourcesFilePath, wheelNode.GetAttributeValue(iconImageAttributeName));

                // Load the texture.
                Texture2D iconImage;
                using (FileStream iconImageStream = File.OpenRead(iconImagePath))
                {
                    iconImage = Texture2D.FromStream(graphicsDevice, iconImageStream);
                }

                // Set the indicator image of the wheel to the loaded texture.
                wheel.Icon = iconImage;
            }

            // Return the created wheel.
            return wheel;
        }
        #endregion
    }
}
