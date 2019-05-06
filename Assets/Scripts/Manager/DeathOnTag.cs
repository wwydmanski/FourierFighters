using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class DeathOnTag : MonoBehaviour
    {
        public string deathTag;

        public string sceneName;
        // Start is called before the first frame update
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag(deathTag))
            {
                StartCoroutine(ReloadLevel());
            }
        }

        private IEnumerator ReloadLevel()
        {
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene(sceneName);
        }

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
