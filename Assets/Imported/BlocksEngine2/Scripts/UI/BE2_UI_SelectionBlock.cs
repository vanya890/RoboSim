using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block;
using UnityEngine.UI;
using static UnityEngine.UI.ContentSizeFitter;
using TMPro;
using MG_BlocksEngine2.Utils;

namespace MG_BlocksEngine2.UI
{
    // v2.10 - BE2_UI_SelectionBlock refactored to improve performance, blocks in the selection viewer are left with only needed components 
    [ExecuteInEditMode]
    public class BE2_UI_SelectionBlock : MonoBehaviour
    {
        public GameObject prefabBlock;

        void OnEnable()
        {
            PerformCleanAndResize();
        }

        public void PerformCleanAndResize()
        {
            if (prefabBlock)
                StartCoroutine(C_PerformCleanAndResize());
        }

        IEnumerator C_PerformCleanAndResize()
        {
            PerformResize();
            yield return new WaitForEndOfFrame();

            // v2.10 - bugfix: variable and list blocks not resizing on the selection panel
            if (!BE2_BlockUtils.BlockIsVariable(prefabBlock.GetComponent<BE2_Block>()))
                PerformClean();
        }

        // v2.10 - selection blocks are cleared of unnecessary components 
        void PerformClean()
        {

            //block
            LayoutGroup lg = transform.GetComponent<LayoutGroup>();
            if (lg) DestroyImmediate(lg);

            transform.GetComponent<RectTransform>().sizeDelta = prefabBlock.transform.GetComponent<RectTransform>().sizeDelta;

            //sections
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child0 = transform.GetChild(i);
                Transform pref0 = prefabBlock.transform.GetChild(i);

                LayoutGroup lg0 = child0.GetComponent<LayoutGroup>();
                if (lg0) DestroyImmediate(lg0);

                if (pref0.GetComponent<BE2_SpotOuterArea>())
                {
                    DestroyImmediate(child0.gameObject);
                }
                else
                {
                    child0.GetComponent<RectTransform>().sizeDelta = pref0.GetComponent<RectTransform>().sizeDelta;
                }

                //header & body
                for (int j = 0; j < child0.childCount; j++)
                {
                    Transform child1 = child0.GetChild(j);
                    Transform pref1 = pref0.GetChild(j);

                    LayoutGroup lg1 = child1.GetComponent<LayoutGroup>();
                    if (lg1) DestroyImmediate(lg1);

                    child1.GetComponent<RectTransform>().sizeDelta = pref1.GetComponent<RectTransform>().sizeDelta;

                    //header components
                    for (int h = 0; h < child1.childCount; h++)
                    {
                        Transform child2 = child1.GetChild(h);
                        Transform pref2 = pref1.GetChild(h);

                        LayoutGroup lg2 = child2.GetComponent<LayoutGroup>();
                        if (lg2) DestroyImmediate(lg2);

                        ContentSizeFitter sf2 = child2.GetComponent<ContentSizeFitter>();
                        if (sf2)
                        {
                            // DestroyImmediate(sf2);
                        }
                        else
                        {
                            child2.GetComponent<RectTransform>().sizeDelta = pref2.GetComponent<RectTransform>().sizeDelta;
                        }

                        Selectable selectable = child2.GetComponent<Selectable>();
                        if (selectable)
                        {
                            selectable.interactable = true;
                        }

                        Image img = child2.GetComponent<Image>();
                        if (img)
                        {
                            img.raycastTarget = false;
                        }

                        BE2_DropdownDynamicResize ddr = child2.GetComponent<BE2_DropdownDynamicResize>();
                        if (ddr)
                        {
                            DestroyImmediate(ddr);
                        }
                        BE2_InputFieldDynamicResize idr = child2.GetComponent<BE2_InputFieldDynamicResize>();
                        if (idr)
                        {
                            DestroyImmediate(idr);
                        }

                        BE2_Dropdown dropdown = BE2_Dropdown.GetBE2Component(child2);
                        if (dropdown != null && !dropdown.isNull)
                        {
                            dropdown.enabled = false;
                            // v2.11 - dropdown arrow from the selection blocks made non raycast target to improve performance
                            Image[] arrows = child2.GetComponentsInChildren<Image>();
                            for (int a = arrows.Length - 1; a >= 0; a--)
                            {
                                arrows[a].raycastTarget = false;
                            }
                        }
                        // v2.10 - using TMP_InputField
                        BE2_InputField inputField = BE2_InputField.GetBE2Component(child2);
                        if (inputField != null && !inputField.isNull)
                        {
                            inputField.enabled = false;
                        }
                    }
                }
            }

            // v2.11 - disable RectMask2D from the selection blocks to improve performance
            RectMask2D[] mask2Ds = gameObject.GetComponentsInChildren<RectMask2D>();
            for (int i = mask2Ds.Length - 1; i >= 0; i--)
            {
                // DestroyImmediate(mask2Ds[i]);
                mask2Ds[i].enabled = false;
            }

            // v2.11 - TMP text from the selection blocks made scaleStatic and non raycast target to improve performance
            TMP_Text[] tmpTexts = gameObject.GetComponentsInChildren<TMP_Text>();
            for (int i = tmpTexts.Length - 1; i >= 0; i--)
            {
                tmpTexts[i].isTextObjectScaleStatic = true;
                tmpTexts[i].raycastTarget = false;
            }
        }

        // v2.10 - selection blocks are adjusted to have the same sizes of the corresponding block
        void PerformResize()
        {
            //sections
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child0 = transform.GetChild(i);
                Transform pref0 = prefabBlock.transform.GetChild(i);

                //header & body
                for (int j = 0; j < child0.childCount; j++)
                {
                    Transform child1 = child0.GetChild(j);
                    Transform pref1 = pref0.GetChild(j);

                    //header components
                    for (int h = 0; h < child1.childCount; h++)
                    {
                        Transform child2 = child1.GetChild(h);
                        Transform pref2 = pref1.GetChild(h);

                        if (child2.GetComponent<Text>() || child2.GetComponent<TMP_Text>())
                        {
                            if (!child2.gameObject.GetComponent<ContentSizeFitter>())
                            {
                                ContentSizeFitter cf = child2.gameObject.AddComponent<ContentSizeFitter>();
                                cf.horizontalFit = FitMode.PreferredSize;
                            }
                        }


                    }
                }
            }
        }
    }
}
