// MIT License
//
// Copyright (c) 2021 Haï~ (@vr_hai github.com/hai-vr)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using UnityEditor;
using UnityEngine;
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
#elif VRC_SDK_VRCSDK2
using VRCAvatarDescriptor = VRCSDK2.VRC_AvatarDescriptor;
#endif

public class DestroyEditorOnly : MonoBehaviour
{
    public bool onlyInsideAvatars;

    private void OnEnable()
    {
        foreach (var rootGo in gameObject.scene.GetRootGameObjects())
        {
            TryDestroyRecursive(rootGo, !onlyInsideAvatars);
        }
    }

    private static void TryDestroyRecursive(GameObject subject, bool destroyPermitted)
    {
        // FindGameObjectsWithTag does not return inactive GameObjects
        if (subject.tag == "EditorOnly" && destroyPermitted)
        {
            Destroy(subject);
        }
        else
        {
#if VRC_SDK_VRCSDK3 || VRC_SDK_VRCSDK2
            var isSubjectAnAvatar = subject.GetComponent<VRCAvatarDescriptor>() != null;
#else
            var isSubjectAnAvatar = false;
#endif
            foreach (Transform child in subject.transform)
            {
                TryDestroyRecursive(child.gameObject, destroyPermitted || isSubjectAnAvatar);
            }
        }
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Enable DestroyEditorOnly")]
    public static void EnableDestroyEditorOnly()
    {
        var go = new GameObject("DestroyEditorOnly");
        go.AddComponent<DestroyEditorOnly>();
    }
#endif
}
