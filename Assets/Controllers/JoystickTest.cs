using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class JoystickTest : MonoBehaviour
{
  public int playerId;
  private Player player;

  int width = 600;
  int height = 300;

  float joystick1Horz;
  float joystick1Vert;
  float joystick2Horz;
  float joystick2Vert;

	/*
  bool fire01 = false;
  bool fire02 = false;
  bool fire03 = false;
  bool fire04 = false;
  bool fire05 = false;
  bool fire06 = false;
  bool fire07 = false;
  bool fire08 = false;
  bool fire09 = false;
  bool fire10 = false;
  bool fire11 = false;
  bool fire12 = false;
*/

  bool up = false;
  bool down = false;
  bool left = false;
  bool right = false;

  bool up2 = false;
  bool down2 = false;
  bool left2 = false;
  bool right2 = false;

  void Awake()
  {
    //player = ReInput.players.GetSystemPlayer();
player = ReInput.players.GetPlayer(playerId);
  }

  void Update()
  {
    joystick1Horz = player.GetAxis("Joystick 1 Horz");
    joystick1Vert = player.GetAxis("Joystick 1 Vert");
    joystick2Horz = player.GetAxis("Joystick 2 Horz");
    joystick2Vert = player.GetAxis("Joystick 2 Vert");

  }

  void OnGUI()
  {
    GUILayout.BeginArea(new Rect(Screen.width/2 - width/2, Screen.height/2 - height/2, width, height));

    // joystick1
    GUILayout.BeginHorizontal();
    GUILayout.Button(up ? "UP" : "^");
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.Button(left ? "LEFT" : "<");
    GUILayout.Button(right ? "RIGHT" : ">");
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.Button(down ? "DOWN" : "v");
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.TextField("Horizontal:" + joystick1Horz);
    GUILayout.TextField("Vertical:" + joystick1Vert);
    GUILayout.EndHorizontal();

    // joystick2
    GUILayout.BeginHorizontal();
    GUILayout.Button(up2 ? "UP" : "^");
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.Button(left2 ? "LEFT" : "<");
    GUILayout.Button(right2 ? "RIGHT" : ">");
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.Button(down2 ? "DOWN" : "v");
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.TextField("Axis 4:" + joystick2Horz);
    GUILayout.TextField("Axis 5:" + joystick2Vert);
    GUILayout.EndHorizontal();

    /*
    GUILayout.BeginHorizontal();
    GUILayout.Button(fire01 ? "FIRE1" : "     ");
    GUILayout.Button(fire02 ? "FIRE2" : "     ");
    GUILayout.Button(fire03 ? "FIRE3" : "     ");
    GUILayout.Button(fire04 ? "FIRE4" : "     ");
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.Button(fire05 ? "FIRE5" : "     ");
    GUILayout.Button(fire06 ? "FIRE6" : "     ");
    GUILayout.Button(fire07 ? "FIRE7" : "     ");
    GUILayout.Button(fire08 ? "FIRE8" : "     ");
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.Button(fire09 ? "FIRE9" : "     ");
    GUILayout.Button(fire10 ? "FIRE10" : "     ");
    GUILayout.Button(fire11 ? "FIRE11" : "     ");
    GUILayout.Button(fire12 ? "FIRE12" : "     ");
    GUILayout.EndHorizontal();
    */

    GUILayout.EndArea();
  }
}
