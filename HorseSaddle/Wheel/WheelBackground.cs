using LiruGameHelper.XML;
using LiruGameHelperMonogame.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Xml;

namespace HorseSaddle.Wheel
{
    /// <summary> The background segments of a <see cref="SpinningWheel"/>. </summary>
    public class WheelBackground
    {
        #region XML Constants
        private const string coloursAttributeName = "Colours";

        private const string borderThicknessAttributeName = "BorderThickness";

        private const string borderColourAttributeName = "BorderColour";

        private const string dividerColourAttributeName = "DividerColour";

        private const string innerRadiusAttributeName = "InnerRadius";
        
        private const string innerColourAttributeName = "InnerColour";

        private const char colourSeparator = ',';
        #endregion

        #region Dependencies
        private readonly GraphicsDevice graphicsDevice;
        #endregion

        #region Fields
        private bool isDirty = false;

        private Color[] colourData = new Color[0];

        private Texture2D backgroundTexture = null;

        private ushort radius = 16;

        private ushort innerRadius = 4;

        private Color innerColour = Color.Black;

        private float borderThickness = 1;

        private Color borderColour = Color.Black;

        private Color? dividerColour = null;

        private int segmentCount;

        private readonly List<Color> segmentColours = new List<Color>() { Color.Black, Color.White };
        #endregion

        #region Properties
        public string Name { get; set; }

        public ushort Radius
        {
            get => radius;
            set
            {
                // If the given value is the same as the current radius or is 0, do nothing.
                if (value == radius || value == 0) return;

                // Set the state to dirty as the texture needs to be redrawn.
                isDirty = true;

                // Set the value.
                radius = value;
            }
        }

        public ushort InnerRadius
        {
            get => innerRadius;
            set
            {
                // Set the state to dirty as the texture needs to be redrawn.
                isDirty = true;

                // Set the value.
                innerRadius = value;
            }
        }

        public Color InnerColour
        {
            get => innerColour;
            set
            {
                // Set the state to dirty as the texture needs to be redrawn.
                isDirty = true;

                // Set the value.
                innerColour = value;
            }
        }

        public Vector2 LocalCentre => backgroundTexture.Bounds.Size.ToVector2() / 2.0f;

        public float BorderThickness
        {
            get => borderThickness;
            set
            {
                // If the given value is the same or invalid, do nothing.
                if (value == borderThickness || value < 0) return;

                // Set the state to dirty as the texture needs to be redrawn.
                isDirty = true;

                // Set the value.
                borderThickness = value;
            }
        }

        public Color BorderColour
        {
            get => borderColour;
            set
            {
                // Set the state to dirty as the texture needs to be redrawn.
                isDirty = true;

                // Set the value.
                borderColour = value;
            }
        }

        public bool DrawDividers => DividerColour.HasValue;

        public Color? DividerColour
        {
            get => dividerColour;
            set
            {
                // Set the state to dirty as the texture needs to be redrawn.
                isDirty = true;

                // Set the value.
                dividerColour = value;
            }
        }

        public IReadOnlyList<Color> SegmentColours
        {
            get => segmentColours;
            set
            {
                // Ensure the given list is valid.
                if (value == null || value.Count < 2) throw new ArgumentException("Given segment colour list was null or had less than 2 colours.");

                // Clear the old list.
                segmentColours.Clear();

                // Add every colour in the given list.
                segmentColours.AddRange(value);

                // Set the state to dirty as the texture needs to be redrawn.
                isDirty = true;
            }
        }

        public int SegmentCount
        {
            get => segmentCount;
            set
            {
                // If the given value is the same or invalid, do nothing.
                if (value == segmentCount || value < 1) return;

                // Set the state to dirty as the texture needs to be redrawn.
                isDirty = true;

                // Set the value.
                segmentCount = value;
            }
        }
        #endregion

        #region Constructors
        public WheelBackground(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        }
        #endregion

        #region Texture Functions
        private void recreateTexture()
        {
            // If the texture does not need to be remade, do nothing.
            if (!isDirty) return;

            // Calculate the size of the new texture, ensuring that it is odd.
            int textureSize = (Radius * 2) + 1;

            // Check to see if the texture needs to be resized or created.
            if (backgroundTexture == null || backgroundTexture.Width != textureSize || backgroundTexture.Height != textureSize)
            {
                // Dispose of the old texture, if it exists.
                backgroundTexture?.Dispose();

                // Create a new texture with the calculated size.
                backgroundTexture = new Texture2D(graphicsDevice, textureSize, textureSize);

                // Resize the colour data array, if needed.
                if (colourData.Length != textureSize * textureSize) Array.Resize(ref colourData, textureSize * textureSize);
            }

            // Calculate the normalised size of a segment.
            float segmentSize = 1.0f / SegmentCount;

            // Go over each pixel within the radius of the wheel.
            for (int x = -Radius; x <= Radius; x++)
                for (int y = -Radius; y <= Radius; y++)
                {
                    // Calculate the 1-dimensional index of the pixel.
                    int pixelIndex = (y + Radius) * textureSize + (x + Radius);

                    // Calculate how far from the circumference of the circle this pixel is, only draw the wheel if this value is equal to or over 0.
                    float distanceFromCircumference = (float)(radius - Math.Sqrt(x * x + y * y));
                    if (distanceFromCircumference >= 0)
                    {
                        // If the distance is within the radius of the inner circle, draw that instead.
                        if (Radius - distanceFromCircumference <= InnerRadius)
                            colourData[pixelIndex] = (InnerRadius - (Radius - distanceFromCircumference) <= BorderThickness) ? BorderColour : InnerColour;
                        // Otherwise; draw the segment.
                        else
                        {
                            // Calculate the angle between this pixel and the centre of the wheel, then normalise it so it's between 0 and 1.
                            float angleScalar = (float)(Math.Atan2(y, x) + Math.PI) / MathHelper.TwoPi;

                            // Calculate the index of this segment.
                            int segmentIndex = (int)Math.Floor(angleScalar / segmentSize);

                            // Place down the colour pixel.
                            colourData[pixelIndex] = (distanceFromCircumference <= BorderThickness) ? BorderColour : SegmentColours[segmentIndex % SegmentColours.Count];
                        }
                    }
                    else colourData[pixelIndex] = Color.Transparent;
                }

            // If dividers are to be drawn, do so.
            if (DrawDividers)
                for (int i = 0; i <= SegmentCount; i++)
                {
                    // Calculate the angle between the two segments.
                    float angle = MathHelper.WrapAngle(((segmentSize * i) + (SegmentCount % 2 != 0 ? segmentSize / 2 : 0)) * MathHelper.TwoPi);

                    // Calculate the direction of the angle and the right of this direction.
                    Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                    // Start on the edge of the inner radius and keep placing pixels until the circumference is reached.
                    Vector2 pixelPos = LocalCentre + (direction * InnerRadius);
                    while (Vector2.DistanceSquared(pixelPos, LocalCentre) <= radius * radius)
                    {
                        // Draw the pixel at this position on the line.
                        colourData[(int)Math.Floor(pixelPos.Y) * textureSize + (int)Math.Floor(pixelPos.X)] = dividerColour.Value;

                        // Increment the position by the direction.
                        pixelPos += direction;
                    }
                }   

            // Set the data of the texture.
            backgroundTexture.SetData(colourData);

            // The texture is no longer dirty.
            isDirty = false;
        }
        #endregion

        #region Segment Functions
        public void AddSegmentColour(Color newColour)
        {
            // Add the colour to the list.
            segmentColours.Add(newColour);

            // Set the state to dirty as the texture needs to be redrawn.
            isDirty = true;
        }
        #endregion

        #region Draw Functions
        public void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, float rotation)
        {
            // If the texture needs to be recreated, do so.
            if (isDirty) recreateTexture();

            // Draw the texture at the given position with the given rotation.
            spriteBatch.Draw(backgroundTexture, screenPosition, null, Color.White, rotation, LocalCentre, 1, SpriteEffects.None, 0);
        }
        #endregion

        #region Load Functions
        public static WheelBackground LoadFromXML(GraphicsDevice graphicsDevice, XmlNode wheelNode)
        {
            // Throw an exception if the given node was null.
            if (wheelNode == null) throw new ArgumentNullException(nameof(wheelNode));

            // Create the basic wheel.
            WheelBackground wheelGraphic = new WheelBackground(graphicsDevice);

            // Load the colours, if any were given.
            if (wheelNode.Attributes.GetNamedItem(coloursAttributeName) is XmlAttribute coloursAttribute)
            {
                // Split the colours into a list of strings and create an array to hold the parsed values.
                string[] colourStrings = coloursAttribute.Value.Split(colourSeparator);
                Color[] segmentColours = new Color[colourStrings.Length];

                // Parse each colour.
                for (int i = 0; i < colourStrings.Length; i++)
                    segmentColours[i] = new Color(Colour.Parse(colourStrings[i]), 1.0f);

                // Set the segment colours to the parsed colours.
                wheelGraphic.SegmentColours = segmentColours;
            }

            // Parse the general settings.
            if (wheelNode.Attributes.GetNamedItem(borderThicknessAttributeName) != null) wheelGraphic.BorderThickness = wheelNode.ParseAttributeValue(borderThicknessAttributeName, float.Parse);
            if (wheelNode.Attributes.GetNamedItem(borderColourAttributeName) != null) wheelGraphic.BorderColour = new Color(wheelNode.ParseAttributeValue(borderColourAttributeName, Colour.Parse), 1.0f);
            if (wheelNode.Attributes.GetNamedItem(dividerColourAttributeName) != null) wheelGraphic.DividerColour = new Color(wheelNode.ParseAttributeValue(dividerColourAttributeName, Colour.Parse), 1.0f);
            if (wheelNode.Attributes.GetNamedItem(innerColourAttributeName) != null) wheelGraphic.InnerColour = new Color(wheelNode.ParseAttributeValue(innerColourAttributeName, Colour.Parse), 1.0f);
            if (wheelNode.Attributes.GetNamedItem(innerRadiusAttributeName) != null) wheelGraphic.InnerRadius = wheelNode.ParseAttributeValue(innerRadiusAttributeName, ushort.Parse);

            // Return the created wheel.
            return wheelGraphic;
        }
        #endregion
    }
}
