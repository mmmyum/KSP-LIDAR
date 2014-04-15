using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[KSPAddon(KSPAddon.Startup.MainMenu, false)]
public class Debug_AutoLoadQuicksaveOnStartup : UnityEngine.MonoBehaviour
{
    public static bool first = true;
    public void Start()
    {
        if (first)
        {
            first = false;
            HighLogic.SaveFolder = "YUMSE";
            var game = GamePersistence.LoadGame("quicksave", HighLogic.SaveFolder, true, false);
            if (game != null && game.flightState != null && game.compatible)
            {
                FlightDriver.StartAndFocusVessel(game, game.flightState.activeVesselIdx);
            }
            //CheatOptions.InfiniteFuel = true;
        }
    }
}

[KSPAddon(KSPAddon.Startup.Flight, false)]
//[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class yumTerrain: MonoBehaviour
    {
          private static Rect windowPosition = new Rect(30,45,130,50);
          private static GUIStyle windowStyle = null;
          private static bool buttonState = false;
          private static Rect windowPosition2 = new Rect(160, 95, 400, 250);
          private static GUIStyle windowStyle2 = null;
          private static bool buttonState2 = false;
          private static bool buttonState3 = false;
          private static bool buttonState4 = false;
          private static bool buttonState5 = false;

          String currentText = "1.0";
          private static double currentValue = 1.0;
          private static double[][] mapVals;
          private static double[][] mapValsLast;
          private static bool debugYUM = true;
          private static string debugYUMString = "YUMSE DEFAULT DEBUG";
          private static int slopeTog = 2; //0 = lat, 1 = long, 2 = elev, 3 = nearestslope


          public void Awake() 
          { 
               RenderingManager.AddToPostDrawQueue(0, OnDraw);
          }

          public void Start() 
          {
               mapValsLast = initMapVals();
               windowStyle = new GUIStyle(HighLogic.Skin.window);
               windowStyle2 = new GUIStyle(HighLogic.Skin.window);
          }

          private void OnDraw()
          {
              windowPosition = GUI.Window(1234, windowPosition, OnWindow, "YUMSE TERRAIN", windowStyle);
              if (buttonState)
              {
                  windowPosition2 = GUI.Window(2345, windowPosition2, OnWindow2, "ELEVATION", windowStyle2);
              }
              if (buttonState2)
              {
                  print("YUMSE SAMPLING: Start");
                  mapValsLast = buildArray();
                  buttonState2 = false;
              }
              if (buttonState3)
              {
                  mapValsLast = initMapVals();
                  buttonState3 = false;
                  print("YUMSE SAMPLING: Restart");
              }
              if (buttonState4)
              {
                  buttonState4 = false;
                  print("YUMSE SAMPLING: Slope Start");
              }
              if (buttonState5)
              {
                  buttonState5 = false;
                  if (slopeTog < 3) slopeTog = 3;
                  else if (slopeTog > 2) slopeTog = 2;
                  print(slopeTog.ToString());
              }
          }

          private void OnWindow(int windowID)
          {
            GUILayout.BeginHorizontal();
            buttonState = GUILayout.Toggle(buttonState, "Enabled" + buttonState);
            GUILayout.EndHorizontal();
            GUI.DragWindow();
          }
          private void OnWindow2(int windowID)
          {
              GUILayout.BeginHorizontal();
              buttonState2 = GUILayout.Toggle(buttonState2, "SAMPLE " + buttonState2);
              buttonState3 = GUILayout.Toggle(buttonState3, "CLEAR " + buttonState3);
              //if (buttonState2)
              //{
              //}
              GUILayout.EndHorizontal();
              //begin display of z values 
              GUILayout.BeginHorizontal();
              GUILayout.Label(mapVals[0][slopeTog].ToString());
              GUILayout.Label(mapVals[1][slopeTog].ToString());
              GUILayout.Label(mapVals[2][slopeTog].ToString());
              GUILayout.EndHorizontal();
              GUILayout.BeginHorizontal();
              GUILayout.Label(mapVals[3][slopeTog].ToString());
              GUILayout.Label(mapVals[4][slopeTog].ToString());
              GUILayout.Label(mapVals[5][slopeTog].ToString());
              GUILayout.EndHorizontal();
              GUILayout.BeginHorizontal();
              GUILayout.Label(mapVals[6][slopeTog].ToString());
              GUILayout.Label(mapVals[7][slopeTog].ToString());
              GUILayout.Label(mapVals[8][slopeTog].ToString());
              GUILayout.EndHorizontal();
              GUILayout.BeginHorizontal();
              GUILayout.Label(mapVals[9][slopeTog].ToString());
              GUILayout.Label(mapVals[10][slopeTog].ToString());
              GUILayout.Label(mapVals[11][slopeTog].ToString());
              GUILayout.EndHorizontal();
              GUILayout.BeginHorizontal();
              GUILayout.Label(mapVals[12][slopeTog].ToString());
              GUILayout.Label(mapVals[13][slopeTog].ToString());
              GUILayout.Label(mapVals[14][slopeTog].ToString());
              GUILayout.EndHorizontal();

              GUILayout.BeginHorizontal();
              GUILayout.Label("Lat Long scaling");
              //try to parse the contents of currentText as a double and put the result in currentValue:
              if (!Double.TryParse(currentText, out currentValue))
              {
                  //the text couldn't be parsed; you'll have to decide what you want to do here
                  currentValue = 1;
              }

              //display the text field and put its current text in currentText
              currentText = GUILayout.TextField(currentText, GUILayout.MinWidth(15.0F)); //you can play with the width of the text box
              GUILayout.EndHorizontal();
              GUILayout.BeginHorizontal();
              buttonState4 = GUILayout.Toggle(buttonState4, "Calculate Slope " + buttonState4);
              buttonState5 = GUILayout.Toggle(buttonState5, "Switch Elevation and Slope " + buttonState5);
              GUILayout.EndHorizontal();
              GUI.DragWindow();
          }
          private static double[][] initMapVals()
          {
              mapVals = new double[][] { new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0},
                                         new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0},
                                         new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0},
                                         new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0},
                                         new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0}};
              if (debugYUM)
              {
                  debugYUMString = String.Format("YUMSE SAMPLING: mapVals initialized {0}", mapVals.Length);
                  print(debugYUMString);
              }
              return mapVals;                   //return mapVals structured to mapvals [15][3], makes a 3x5 grid
          }
          private static double[][] buildArray()
          {
              Vessel vessel = FlightGlobals.ActiveVessel;
              Vector3 pos = vessel.findLocalCenterOfMass();
              double alt = vessel.mainBody.GetAltitude(pos);
              //double radius = vessel.mainBody.Radius;
              string body = vessel.mainBody.name;
              int index1 = 0;
              double curX = vessel.mainBody.GetLatitude(pos);
              double curY = vessel.mainBody.GetLongitude(pos);

              double xOffset = currentValue;
              double yOffset = xOffset + xOffset;
              double maxX = curX + xOffset;
              double minX = curX - xOffset;
              double maxY = curY + yOffset;
              double minY = curY - yOffset;
              //double elev = vessel.mainBody.pqsController.GetSurfaceHeight(pos); // - radius
              //double curZ = elev;

              var surfH = vessel.mainBody.pqsController.GetSurfaceHeight(QuaternionD.AngleAxis(curY, Vector3d.down) * QuaternionD.AngleAxis(curX, Vector3d.forward) * Vector3d.right) - vessel.mainBody.pqsController.radius;
              double curZ = surfH;

              if (debugYUM)
              {
                  debugYUMString = String.Format("YUMSE SAMPLING: BUILD ARRAY START {0} {1} {2} {3}", curX, curY, curZ, surfH);
                  print(debugYUMString);
              }
              //for loops to construct mapVals from original position 
              for (curY = maxY; curY > minY - yOffset; curY = curY - xOffset)
              {
                  for (curX = minX; curX < maxX + xOffset; curX = curX + xOffset)
                  {
                      //code to get Z (elevation) from Lat/Long
                      surfH = vessel.mainBody.pqsController.GetSurfaceHeight(QuaternionD.AngleAxis(curY, Vector3d.down) * QuaternionD.AngleAxis(curX, Vector3d.forward) * Vector3d.right) - vessel.mainBody.pqsController.radius;
                      curZ = surfH;

                      mapVals[index1][0] = curX;
                      mapVals[index1][1] = curY;
                      mapVals[index1][2] = curZ;
                      if (debugYUM)
                      {
                          debugYUMString = String.Format("YUMSE SAMPLING: {0} , {1} , {2} at {3}", curX, curY, curZ, index1);
                          print(debugYUMString);
                      }
                      index1++;
                      if (index1 > 14) break;
                  }
                  if (index1 > 14) break;
              }
              return mapVals;
          }
    }