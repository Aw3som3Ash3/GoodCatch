using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    // Public variables
    public AudioSource src; // Gameobject's Audio Source component.
    public AudioClip[] clip; // Custom array for audio clips; use the editor to insert any number of sound effects to randomly play per given action.

    // Private variables
    private AudioClip activeClip;

    // To play an audio clip, insert the line "audioController.PlayClipRandom();" into the Input Action of the source object's script. Remember to include a reference to the Audio Source with this script in the former's own code, preferably with a public declaration "public AudioController audioController;".
    public void PlayClipRandom()
    {
        // Sets random active clip from array.
        activeClip = clip[Random.Range(0, clip.Length)];

        // Plays chosen clip.
        src.PlayOneShot(activeClip);
    }
}
