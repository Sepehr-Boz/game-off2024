using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Dictionary<Vector2, GameObject> levelPositions;

    [SerializeField] private Transform player;

    void Start()
    {
        // loop through every level template and get their position, to then check and enable when it should be enabled
        // or disabled

        GameObject grid = null;
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name == "Grid")
            {
                grid = root;
                break;
            }
        }

        levelPositions = new();
        foreach (Transform template in grid.transform)
        {
            if (template.name.Contains("Level"))
                levelPositions.Add(template.position, template.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -10);

        // loop through every position and check if it is in view or not
        foreach (KeyValuePair<Vector2, GameObject> level in levelPositions)
        {
            Vector2 vpPos = GetComponent<Camera>().WorldToViewportPoint(level.Key);
            // print(vpPos);

            if (vpPos.x < -0.15f || vpPos.x > 1.15f || vpPos.y < -0.15f || vpPos.y > 1.15f)
            {
                level.Value.SetActive(false);
            }
            else
            {
                level.Value.SetActive(true);
            }
        }
    }
}
