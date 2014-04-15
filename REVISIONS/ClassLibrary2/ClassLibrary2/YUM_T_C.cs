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
          private static Rect windowPosition2 = new Rect(160, 95, 600, 450);
          private static GUIStyle windowStyle2 = null;
          private static bool buttonState2 = false;
          private static bool buttonState3 = false;
          private static bool buttonState4 = false;
          private static bool buttonState5 = false;

          String currentText = "0.15";
          private static double currentValue = 0.15;
          private static double[][] mapVals;
          private static double[][] mapValsLast;
          private static bool debugYUM = true;
          private static string debugYUMString = "YUMSE DEFAULT DEBUG";
          private static int slopeTog = 2; //0 = lat, 1 = long, 2 = elev, 3 = nearestslope
          private static int resX = 5;
          private static int resY = 7;
          String curResX = "5";
          String curResY = "7";
          private static int index = 0;
          //private static double cellResolutionMeters = 0;

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
                  calculateAllSlopes();
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
            buttonState = GUILayout.Toggle(buttonState, "Enabled " + buttonState);
            GUILayout.EndHorizontal();
            GUI.DragWindow();
          }
          private void OnWindow2(int windowID)
          {
              GUILayout.BeginHorizontal();
              buttonState2 = GUILayout.Toggle(buttonState2, "SAMPLE ");
              buttonState3 = GUILayout.Toggle(buttonState3, "CLEAR ");
              //if (buttonState2)
              //{
              //}
              GUILayout.EndHorizontal();
              //begin display of z values 
              index = 0;
              //while (index < mapVals.Length - 1)
              //{
                  for (int i = 0; i < resY; i++)
                  {
                      if (index > mapVals.Length - 1) break;
                      GUILayout.BeginHorizontal();
                      for (int j = 0; j < resX; j++)
                      {
                          if (index > mapVals.Length - 1) { GUILayout.EndHorizontal(); break; }
                          GUILayout.Label(mapVals[index][slopeTog].ToString());
                          index++;
                      }
                      GUILayout.EndHorizontal();
                  }
              //}
              //method 1, not dynamic
              /*
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
              */

              GUILayout.BeginHorizontal();
              GUILayout.Label("X Resolution");
              if (!int.TryParse(curResX, out resX))
              {
                  resX = 5;
              }
              curResX = GUILayout.TextField(curResX, GUILayout.MinWidth(15.0F));
              GUILayout.EndHorizontal();

              GUILayout.BeginHorizontal();
              GUILayout.Label("Y Resolution");
              if (!int.TryParse(curResY, out resY))
              {
                  resY = 7;
              }
              curResY = GUILayout.TextField(curResY, GUILayout.MinWidth(15.0F));
              GUILayout.EndHorizontal();

              GUILayout.BeginHorizontal();
              GUILayout.Label("Lat Long Scaling");
              if (!Double.TryParse(currentText, out currentValue))
              {
                  currentValue = 1;
              }
              currentText = GUILayout.TextField(currentText, GUILayout.MinWidth(15.0F)); 
              GUILayout.EndHorizontal();

              GUILayout.BeginHorizontal();
              buttonState4 = GUILayout.Toggle(buttonState4, "Calculate Slope ");
              buttonState5 = GUILayout.Toggle(buttonState5, "Switch Elevation and Slope ");
              GUILayout.EndHorizontal();
              GUI.DragWindow();
          }
          private static double[][] initMapVals()
          {
              int cnt = 0;
              int num = resX * resY;
              mapVals = new double[num][];
              for (cnt = 0; cnt < (resX * resY); cnt++)
              {
                  mapVals[cnt] = new double[4];
              }
              //old system
              /*
              mapVals = new double[][] { new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0},
                                         new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0},
                                         new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0},
                                         new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0},
                                         new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0}, new double[] {0.0,0.0,0.0,0.0}};
              
              */
              if (debugYUM)
              {
                  debugYUMString = String.Format("YUMSE SAMPLING: mapVals initialized {0}", mapVals.Length);
                  print(debugYUMString);
              }
              return mapVals;                   //return mapVals
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


              double xyOffset = currentValue; //dist between measurements
              double halfResX = resX / 2;
              double halfResY = resY / 2;

              double xFactor = Math.Floor(halfResX);
              double yFactor = Math.Floor(halfResY);

              if (debugYUM)
              {
                  //double xyOffsetFactor = xyOffset * ((resX / 2) - 0.5); 
                  print("YUMSE DEBUG: Offset Factor X " + xFactor + " Y " + yFactor);
                  //print(xyOffsetFactor);
              }
              //maxX should be curX + n Offsets to get range = 
              //n should be 1 offset at 3, 2 at 5, 3 at 7, 4 at 9
              //going to have to scrub values to ony give odd by odd numbers to maintain consistency
              double maxX = curX + (xyOffset * xFactor);
              double maxY = curY + (xyOffset * yFactor);
              double minX = curX - (xyOffset * xFactor);
              double minY = curY - (xyOffset * yFactor);

              //old system
              /*
              double xOffset = currentValue;
              double yOffset = xOffset + xOffset;
              double maxX = curX + xOffset;
              double minX = curX - xOffset;
              double maxY = curY + yOffset;
              double minY = curY - yOffset;
              //double elev = vessel.mainBody.pqsController.GetSurfaceHeight(pos); // - radius
              //double curZ = elev;
              */

              var surfH = vessel.mainBody.pqsController.GetSurfaceHeight(QuaternionD.AngleAxis(curY, Vector3d.down) * QuaternionD.AngleAxis(curX, Vector3d.forward) * Vector3d.right) - vessel.mainBody.pqsController.radius;
              double curZ = surfH;

              if (debugYUM)
              {
                  debugYUMString = String.Format("YUMSE SAMPLING: BUILDARRAY START {0} {1} {2} {3}", curX, curY, curZ, surfH);
                  print(debugYUMString);
                  debugYUMString = String.Format("YUMSE SAMPLING: BUILDARRAY MINS {0} {1}", minX, minY);
                  print(debugYUMString);
                  debugYUMString = String.Format("YUMSE SAMPLING: BUILDARRAY MAXS {0} {1}", maxX, maxY);
                  print(debugYUMString);
              }
              //for loops to construct mapVals from original position 
              for (curY = maxY; curY != minY - xyOffset; curY = curY - xyOffset)
              {
                  for (curX = minX; curX != maxX + xyOffset; curX = curX + xyOffset)
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
                      if (index1 > (resX * resY) - 1) { print("YUMSE: ERROR: index1 out of bounds"); break; }
                  }
                  if (index1 > (resX * resY) - 1) { print("YUMSE: ERROR: index1 out of bounds"); break; }
              }
              return mapVals;

          }

          private void calculateAllSlopes()
          {
              print("YUMSE: SLOPE CALCULATION BEGINS");
              for (int i = 0; i < mapVals.Length; i++)
              {
                  double curCellSlope = getCellSlope(i);
                  print(curCellSlope);
              }
          }

          private static double getCellSlope(int indexT)
          {
              //first get neighbouring cells - 
              int[] neighbourCellIndex = getNeighbourCells(indexT);
              //now we need to check to make sure all the index's called are within range.
              int tempVal = 0;
              for (int leftSide = 0; leftSide < resY; leftSide++)
              {
                  tempVal = tempVal + resX;
                  if (indexT == tempVal) return 0;
              }
              tempVal = resX - 1;
              for (int rightSide = 0; rightSide < resY; rightSide++)
              {
                  tempVal = tempVal + resX;
                  if (indexT == tempVal) return 0;
              }
              //old system
              /*
              foreach (int i in neighbourCellIndex)
              {
                  //Old if statements to prevent error
                  if (i < 0) return 0;
                  if (i > mapVals.Length - 1) return 0;
                  //if (i == 0) return 0;
                  int tempVal = 0;
                  for (int leftSide = 0; leftSide < resY; leftSide++)
                  {
                      tempVal = tempVal + resX;
                      if (i == tempVal) return 0;
                  }
                  tempVal = resX - 1;
                  for (int rightSide = 0; rightSide < resY; rightSide++)
                  {
                      tempVal = tempVal + resX;
                      if (i == tempVal) return 0;
                  }
                  //if (i + resX + 1 > mapVals.Length -1 ) return 0;
              }
              */

              //here we'll use a try catch
              try
              {
                  //then get delta X and delta Y
                  int indexMapVals = neighbourCellIndex[0];
                  double initLat = mapVals[indexMapVals][0];
                  double initLng = mapVals[indexMapVals][1];
                  indexMapVals = neighbourCellIndex[1];
                  double yLat = mapVals[indexMapVals][0];
                  double yLng = mapVals[indexMapVals][1];
                  indexMapVals = neighbourCellIndex[4];
                  double xLat = mapVals[indexMapVals][0];
                  double xLng = mapVals[indexMapVals][1];


                  //now call cellSizeMeters twice to calculate the cell size in x and y
                  double cellSizeX = cellSizeMeters(initLat, initLng, xLat, xLng);
                  double cellSizeY = cellSizeMeters(initLat, initLng, yLat, yLng);
                  //delta x and delta y are approximated by doubling cellsizeX and cellsizeY
                  if (debugYUM)
                  {
                      print("YUMSE: Cell Size " + cellSizeX + " by " + cellSizeY);
                      print("YUMSE: Neighbouring Cells Index ");
                      foreach (int item in neighbourCellIndex)
                      {
                          print(item.ToString());
                      }
                  }
                  //
                  //
                  //so now we can calculate the slope!
                  //
                  try
                  {
                      //begin with the x axis (delta z/ delta x)
                      double deltaZx = (((mapVals[neighbourCellIndex[6]][2] - mapVals[neighbourCellIndex[5]][2]) / (cellSizeX * 2)) +
                                        ((mapVals[neighbourCellIndex[4]][2] - mapVals[neighbourCellIndex[3]][2]) / (cellSizeX * 2)) +
                                        ((mapVals[neighbourCellIndex[8]][2] - mapVals[neighbourCellIndex[7]][2]) / (cellSizeX * 2))) / 3;
                      double deltaZy = (((mapVals[neighbourCellIndex[5]][2] - mapVals[neighbourCellIndex[7]][2]) / (cellSizeY * 2)) +
                                        ((mapVals[neighbourCellIndex[1]][2] - mapVals[neighbourCellIndex[2]][2]) / (cellSizeY * 2)) +
                                        ((mapVals[neighbourCellIndex[6]][2] - mapVals[neighbourCellIndex[8]][2]) / (cellSizeY * 2))) / 3;
                      double cellSlope = Math.Sqrt((Math.Pow(deltaZx, 2)) + (Math.Pow(deltaZx, 2)));
                      mapVals[indexT][3] = cellSlope;
                      return cellSlope;
                  }
                  catch (System.IndexOutOfRangeException ex)
                  {
                      print("YUMSE: Index Exception Catch at Calculate slope");
                      print(ex);
                      return 0;
                  }
              }
              catch (System.IndexOutOfRangeException ex)
              {
                  print("YUMSE: Index Exception Catch at get deltaX and get deltaY");
                  print(ex);
                  return 0;
              }
          }
          
          private static int[] getNeighbourCells(int iT)
          {
              //array layout 0 = iT 1 = iTN 2 = iTP 3 = iTN1 4 = iTP1 5 = iTNN1 6 = iTNP1 7 = iTPN1 8 = iTPP1
              int[] neighBourCellIndex = new int[9];
              neighBourCellIndex[0] = iT;
              neighBourCellIndex[1] = iT - resX;
              neighBourCellIndex[2] = iT + resX;
              neighBourCellIndex[3] = iT - 1;
              neighBourCellIndex[4] = iT + 1;
              neighBourCellIndex[5] = (iT - resX) - 1;
              neighBourCellIndex[6] = (iT - resX) + 1;
              neighBourCellIndex[7] = (iT + resX) - 1;
              neighBourCellIndex[8] = (iT + resX) + 1;

              return neighBourCellIndex;
          }

        private static double cellSizeMeters (double lat1, double lng1, double lat2, double lng2)
        {
            //this should return the cell size in meters, to be called by the slope method
            double cellSizeM;
            Vessel vessel = FlightGlobals.ActiveVessel;
            double radius = vessel.mainBody.Radius;
            cellSizeM = 0;

            double earthRadius = radius; //3958.75;
            double dLat = ToRadians(lat2 - lat1);
            double dLng = ToRadians(lng2 - lng1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double dist = earthRadius * c;
            double meterConversion = 1609.00;
            cellSizeM = dist * meterConversion;

            return cellSizeM;
        }
        private static double ToRadians(double degrees)
        {
            double radians = degrees * Math.PI / 180;
            return radians;
        }
    }