using System;
using System.Collections.Generic;

namespace Kineckt {
    public class Scene {
        public Camera Camera { get; set; }
        public Sun Sun { get; set; }
        public List<GameObject> GameObjects { get; } = new List<GameObject>();


        public void Spawn(GameObject go) {
            // I dont know what i did here...
            switch (go) {
                case Sun _ when Sun != null:
                    throw new Exception("You can only spawn one Sun");
                case Sun sun:
                    Sun = sun;
                    break;
                case Camera _ when Camera != null:
                    throw new Exception("You can only spawn one Camera");
                case Camera cam:
                    Camera = cam;
                    break;
            }

            GameObjects.Add(go);
        }
    }
}