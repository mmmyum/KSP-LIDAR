using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//thanks to NavyFish for gui code, thanks to faark for dev module[ed
[KSPAddon(KSPAddon.Startup.MainMenu, false)]
public class Debug_AutoLoadQuicksaveOnStartup : UnityEngine.MonoBehaviour
{
    public static bool first = true;
    public void Start()
    {
        if (first)
        {
            first = false;
            HighLogic.SaveFolder = "test";
            var game = GamePersistence.LoadGame("quicksave", HighLogic.SaveFolder, true, false);
            if (game != null && game.flightState != null && game.compatible)
            {
                FlightDriver.StartAndFocusVessel(game, game.flightState.activeVesselIdx);
            }
            CheatOptions.InfiniteFuel = true;
        }
    }
}


[KSPAddon(KSPAddon.Startup.Flight, false)]
    public class YUMSEmodule : MonoBehaviour
    {
          private static double[][] mapVals;
          private static double curX, maxX, minX, curY, maxY, minY, curZ;
          private static int index1 = 0;
          private static Rect windowPosition = new Rect(0,0,320,320);
          private static GUIStyle windowStyle = null;
          private static bool buttonState = false;
          private static bool updateInfo = false;
          private static Vessel vessel = FlightGlobals.ActiveVessel;
          static Vector3 pos = vessel.findLocalCenterOfMass();
          double alt = vessel.mainBody.GetAltitude(pos);
          double radius = vessel.mainBody.Radius;
          string body = vessel.mainBody.name;
          double curLat = vessel.mainBody.GetLatitude(pos);
          double curLong = vessel.mainBody.GetLongitude(pos);

          public void Awake() { 
               RenderingManager.AddToPostDrawQueue(0, OnDraw);
          }

          public void Start() {
              //mapVals = initMapVals(mapVals);
              windowStyle = new GUIStyle(HighLogic.Skin.window);
          }

          private void OnDraw()
          {
              windowPosition = GUI.Window(1234, windowPosition, OnWindow, "YUMSE FLIGHT SYSTEM", windowStyle);
          }

          private void OnWindow(int windowID){
            GUILayout.BeginHorizontal();
            GUILayout.Label("ELEV AND SLOPE");
            GUILayout.Label("MAYBE STATS");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            buttonState = GUILayout.Toggle(buttonState, "Show Surface Info: " + buttonState);
            GUILayout.EndHorizontal();
            if (buttonState)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(body);
                GUILayout.Label(pos.ToString());
                GUILayout.Label(alt.ToString());
                GUILayout.EndHorizontal();
                //begin display of z values
                GUILayout.BeginHorizontal();
                GUILayout.Label(mapVals[0][2].ToString());
                GUILayout.Label(mapVals[1][2].ToString());
                GUILayout.Label(mapVals[2][2].ToString());
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label(mapVals[3][2].ToString());
                GUILayout.Label(mapVals[4][2].ToString());
                GUILayout.Label(mapVals[5][2].ToString());
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label(mapVals[6][2].ToString());
                GUILayout.Label(mapVals[7][2].ToString());
                GUILayout.Label(mapVals[8][2].ToString());
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label(mapVals[9][2].ToString());
                GUILayout.Label(mapVals[10][2].ToString());
                GUILayout.Label(mapVals[11][2].ToString());
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label(mapVals[12][2].ToString());
                GUILayout.Label(mapVals[13][2].ToString());
                GUILayout.Label(mapVals[14][2].ToString());
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            updateInfo = GUILayout.Toggle(updateInfo, "RECALCULATE " + updateInfo);
            GUILayout.EndHorizontal();
            if (updateInfo)
            {
                updateInfo = false;
                printArray();
            }
           GUI.DragWindow();
          }
        
    
    //begin yum calc code
       /********************************************************************************
       * TODO
       * 
       *   
       *  set curZ to ground elevation 
       *  button to call array builderXY
       *  
       *  1ST --- HOW to get Z values for offset ship position - i.e. transform get elevation from lat long values
       *  Display data - print rows of labels
       *  Next step - calculate slope values (makes 4x2 grid)
       * 
       * *******************************************************************************/
          private static double[][] initMapVals(double[][] mapVals)
          {
              mapVals = new double[][] { new double[] {0.0,0.0,0.0}, new double[] {0.0,0.0,0.0}, new double[] {0.0,0.0,0.0},
                   new double[] {0.0,0.0,0.0}, new double[] {0.0,0.0,0.0}, new double[] {0.0,0.0,0.0},
                    new double[] {0.0,0.0,0.0}, new double[] {0.0,0.0,0.0}, new double[] {0.0,0.0,0.0},
                     new double[] {0.0,0.0,0.0}, new double[] {0.0,0.0,0.0}, new double[] {0.0,0.0,0.0},
                      new double[] {0.0,0.0,0.0}, new double[] {0.0,0.0,0.0}, new double[] {0.0,0.0,0.0}};
              return mapVals;                   //return mapVals structured to mapvals [15][3], makes a 5 x 3 grid
          }

          private void arrayBuilderXY(double curLat, double curLong)
          {
              pos = vessel.findLocalCenterOfMass();
		      alt = vessel.mainBody.GetAltitude(pos);
		      radius = vessel.mainBody.Radius;
		      body = vessel.mainBody.name;
              index1 = 0;
              curX = curLat;
              curY = curLong;
              maxX = curX + 2;
              minX = curX - 2;
              maxY = curY + 1;
              minY = curY - 1;
              double elev = vessel.mainBody.pqsController.GetSurfaceHeight(pos) - radius;
              curZ = elev;
              mapVals = initMapVals(mapVals);
              
              //for loops to construct mapVals from original position
              for (curX = maxX; curX == minX; curX--)
              {
                  for (curY = minY; curY == maxY; curY++)
                  {
                      //INSERT -- code to get Z (elevation) from Lat/Long
                      mapVals[index1][0] = curX;
                      mapVals[index1][1] = curY;
                      mapVals[index1][2] = curZ;
                  }
                  index1++;
              }
          }
            
          //summing arrays
          private static int[] AddArrays(int[] a, int[] b)
          {
              return a.Zip(b, (x, y) => x + y).ToArray();
          }

          //protected override void onPartUpdate()
          private void printArray() 
          {
              double curLat = vessel.mainBody.GetLatitude(pos);
              double curLong = vessel.mainBody.GetLongitude(pos);
              arrayBuilderXY(curLat, curLong);
          }
    }