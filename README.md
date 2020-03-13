# UI Image Alpha Test (Unity / C#)

![UI Image alpha test](/doc/ui_image_alpha_test.gif)

## What is it?

This is a uGUI system related script I did a while back. By default Unity UI elements don't check user clicks based on UI element image opaque pixels, elements only check if user clicked inside the RectTransform of that specific UI element. Luckily Unity has provided an interface that can be used to replace the default behaviour. Add a script with this interface to your you GameObject with a UI Image component.

### Features:

* Works with Simple type UI image

* Works with Simple type UI image that has "Preserve Aspect" toggled on (using Aspect ratio fitter)

* Works with Sliced type UI image (9-slice sprite)

* Works with rotated, scaled and resized RectTransforms


## About UI System and Sprites
Unity UI hit test with alpha test is not very straight forward to create. First of all there can textures with a single Sprite or there can be textures with several sprites like seen here:

![UI Image alpha test](/doc/ui_sprite_sheet.PNG)

This results in the fact that there is no direct mapping from sprite texture to UI Rect. Sprite sub-rectangle and its relative position from botton left corner of texture UV square must be taken care of. 

Also, UI element RectTransform might not be the same size as sprite, so when a click is converted to RectTransform space, it must be then converted to a local position in sprite rectangle. 

And then there's the pivot - UI allows UI element pivot to be moved, so pivot must be taken into account - as it alters rectTransform position values. Pivot's position influence to RectTransform origin can be removed and that way coordinates start from the bottom left corner of a RectTransform. After this sprite's pixel values can be read.

Sliced mode Image is a bit more complicated case. There are the sides formed by left, right, bottom and top margins (each of which can have unique width value) and the center area (actually a cross shaped area). Corners don't stretch but the areas between corners and center area of 9-slice will.


## Limitations

* Requires images to be marked as Read/Write enabled

* Requires images to be uncompressed (?)


# Classes

## RaycastAlphaMask.cs
The class implementing ICanvasRaycastFilter and its method IsRaycastLocationValid. IsRaycastLocationValid calls methods that will actually do the checking depending on the Image type, i.e. Simple type or Sliced type Image.

# About
I created this UI alpha hit test class for myself, as a learning experience and it was supposed to be used for different personal Unity projects - although I think there are now better alternatives available in the Asset Store.

# Copyright 
Created by Sami S. use of any kind without a written permission from the author is not allowed. But feel free to take a look.
