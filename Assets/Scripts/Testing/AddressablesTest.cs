using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesTest : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(
            "SFX/new notification 1.wav"
        );

        while (!handle.IsDone)
        {
            Debug.Log(handle.PercentComplete);
            yield return new WaitForEndOfFrame();
        }

        AudioClip clip = null;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            clip = handle.Result;
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position + Vector3.forward);
            Addressables.Release(handle);
        }
        else
        {
            Addressables.Release(handle);
            throw new UnityException("Could not find file new notification 1");
        }
    }
}
