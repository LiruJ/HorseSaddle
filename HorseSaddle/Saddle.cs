using GuiCookie;
using GuiCookie.Rendering;
using HorseSaddle.Gui;
using HorseSaddle.Teams;
using HorseSaddle.Units;
using HorseSaddle.Wheel;
using HorseSaddle.Wheel.Segments;
using LiruGameHelper.XML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace HorseSaddle
{
    public class Saddle : Game
    {
        #region XML Constants
        private const string wheelPresetAttributeName = "WheelName";

        private const string pointsPerRoundAttributeName = "PointsPerRound";

        private const string scoreNameAttributeName = "ScoreName";
        #endregion

        #region Dependencies
        private readonly Random random = new Random();

        private readonly GraphicsDeviceManager graphics;
        #endregion

        #region Game Fields
        private readonly Dictionary<string, Unit> unitsByName = new Dictionary<string, Unit>();

        private int currentRound = 0;

        private int pointsPerRound = 2500;
        #endregion

        #region Graphical Fields
        private UIManager uiManager;

        private WheelsController wheelsController = null;

        private GuiCamera guiCamera = null;

        #endregion

        #region Game Properties
        public string ScoreName { get; private set; }

        public int CurrentRound
        {
            get => currentRound; 
            set
            {
                // Set the current round
                currentRound = MathHelper.Clamp(value, 0, MaxScore * 2);

                // Set the base budget for both teams.
                LeftTeam.BasePoints = pointsPerRound * (currentRound + 1);
                RightTeam.BasePoints = pointsPerRound * (currentRound + 1);
            }
        }

        public int MaxScore => ScoreName == null ? 0 : ScoreName.Length;

        public string WheelPresetPath { get; private set; }

        public IReadOnlyDictionary<string, Unit> UnitsByName => unitsByName;

        public Team RightTeam { get; private set; } = null;

        public Team LeftTeam { get; private set; } = null;
        #endregion

        #region Constructors
        public Saddle()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #endregion

        #region Initialisation Functions
        protected override void Initialize()
        {
            // Create the UI manager.
            uiManager = new UIManager(this);
            uiManager.RegisterElementNamespace(System.Reflection.Assembly.GetExecutingAssembly(), "HorseSaddle.Gui.Elements");

            // Initialise to 720p.
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            // Some QoL stuff.
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            // Call the base initialisation function.
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Load the main font.
            SpriteFont wheelFont = Content.Load<SpriteFont>("WheelFont");

            // Load the main file, which contains the wheel to be loaded as well as team info.
            XmlDocument settingsFile = new XmlDocument();
            settingsFile.Load("Settings.xml");
            XmlNode settingsNode = settingsFile.LastChild;

            // Set the settings.
            pointsPerRound = settingsNode.ParseAttributeValue(pointsPerRoundAttributeName, int.Parse);
            ScoreName = settingsNode.GetAttributeValue(scoreNameAttributeName);
            WheelPresetPath = Path.Combine("WheelPresets", settingsNode.GetAttributeValue(wheelPresetAttributeName), "Wheels.xml");

            // Load the units.
            XmlDocument unitsFile = new XmlDocument();
            unitsFile.Load(Path.Combine(Path.GetDirectoryName(WheelPresetPath), "Units.xml"));
            foreach (XmlNode unitNode in unitsFile.LastChild)
            {
                Unit unit = Unit.LoadFromXMLNode(unitNode);
                unitsByName.Add(unit.Name, unit);
            }

            // Load the teams.
            LeftTeam = Team.LoadFromXMLNode(settingsNode.SelectSingleNode("LeftTeam"), this);
            RightTeam = Team.LoadFromXMLNode(settingsNode.SelectSingleNode("RightTeam"), this);

            // Load the wheels and randomise the rotations.
            List<SpinningWheel<Segment>> wheels = SpinningWheel<Segment>.LoadWheelsFromXML(GraphicsDevice, this, wheelFont, WheelPresetPath);
            foreach (SpinningWheel<Segment> wheel in wheels)
                wheel.RandomiseRotation(random);

            // Create the gui camera for the UI.
            guiCamera = uiManager.CreateGuiCamera();
            guiCamera.BlendState = BlendState.NonPremultiplied;

            // Create the UI and set the teams.
            wheelsController = uiManager.CreateUIRoot<WheelsController>(Path.Combine(Content.RootDirectory, "Gui", "Wheels.xml"), wheels, random, this);
            wheelsController.UnitSelectorWindow.PopulateFromUnits(unitsByName);
            wheelsController.LeftTeamDisplay.Team = LeftTeam;
            wheelsController.RightTeamDisplay.Team = RightTeam;

            CurrentRound = 0;
        }
        #endregion

        #region Update Functions
        protected override void Update(GameTime gameTime)
        {
            // Update the UI.
            wheelsController.Update(gameTime);
            
            // Update the base.
            base.Update(gameTime);
        }
        #endregion

        #region Draw Functions
        protected override void Draw(GameTime gameTime)
        {
            // Clear to the setting's colour.
            GraphicsDevice.Clear(Color.Magenta);

            // Draw the UI.
            guiCamera.Begin();
            wheelsController.Draw(guiCamera);
            guiCamera.End();

            // Draw the base.
            base.Draw(gameTime);
        }
        #endregion
    }
}
