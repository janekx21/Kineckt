using System;
using System.Collections.Generic;
using Kineckt.GameObjects;

namespace Kineckt.Engine {
    public class Scene {
        public Camera Camera { get; private set; }
        public Sun Sun { get; private set; }
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

        public void Destroy(GameObject go) {
            if (GameObjects.Remove(go)) go.OnDie();

            switch (go) {
                case Sun _:
                    Sun = null;
                    break;
                case Camera _:
                    Camera = null;
                    break;
            }
        }

        public T GetFirstOrNull<T>() where T : GameObject {
            return GameObjects.Find(o => o is T) as T;
        }

        public void Reset() {
            Sun = null;
            Camera = null;

            GameObjects.Clear();
        }
    }
}