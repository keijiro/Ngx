# Ngx - Neural network based visual generator and mixer

**Ngx** is an attempt at utilizing a neural network for VJing. It implements
[pix2pix] (image-to-image translation with cGAN) as an ad-hoc next-frame
prediction model that is trained with pairs of consecutive frames extracted
from a video clip, so that it can generate an image sequence for an infinite
duration just by repeatedly feeding frames back. It also has functionality for
mixing (crossfading) two pix2pix models that gives unexpected variation and
transition to generated video.

![gif](https://i.imgur.com/2WMKfdA.gif)
![gif](https://i.imgur.com/sMgvbo6.gif)

(The gif on the right is from a tweet by [@chaosgroove].)

[Vimeo - Ngx demonstration](https://vimeo.com/294399440)

[pix2pix]: https://phillipi.github.io/pix2pix/
[@chaosgroove]: https://twitter.com/chaosgroove/status/1051082769843990528

Pre-built executables
---------------------

You can download a pre-built executable from [Releases] page. It contains
pre-trained pix2pix models with [Beeple]'s VJ clips.

| Model Name | Original clip  | Vimeo link                  |
| ---------- | -------------- | --------------------------- |
| Beeple 1   | FIBER OPTICAL  | https://vimeo.com/238083470 |
| Beeple 2   | redgate.v1     | https://vimeo.com/106577855 |
| Beeple 3   | p-crawl        | https://vimeo.com/116330714 |
| Beeple 4   | exhaust        | https://vimeo.com/95513505  |
| Beeple 5   | quicksilver    | https://vimeo.com/153493904 |
| Beeple 6   | MOONVIRUS      | https://vimeo.com/155970396 |
| Beeple 7   | TENDRIL        | https://vimeo.com/158477477 |
| Beeple 8   | shifting pains | https://vimeo.com/48818536  |

The original video clips are licensed under a Creative Commons Attribution
license ([CC-BY 3.0]). These pre-trained models are also attributed to the
original author.

[Releases]: https://github.com/keijiro/Ngx/releases
[Beeple]: https://www.beeple-crap.com/
[CC-BY 3.0]: https://creativecommons.org/licenses/by/3.0/

How to control
--------------

Ngx can be controlled with the on-screen controller or a MIDI controller.

| Name      | Description                       | MIDI CC |
| --------- | --------------------------------- | ------- |
| Mix       | Crossfading between model 1 and 2 | 77      |
| Noise 1   | Noise injection (low density)     | 78      |
| Noise 2   | Noise injection (medium density)  | 79      |
| Noise 3   | Noise injection (high density)    | 80      |

| Name      | Description         | MIDI CC |
| --------- | ------------------- | ------- |
| Feedback  | Feedback rate       | 81, 82  |
| FColor    | False color effect  | 83      |
| Hue Shift | Amount of hue shift | 55, 56  |
| Invert    | Color inversion     | 84      |

| Name           | MIDI notes                     |
| -------------- | ------------------------------ |
| Model Select 1 | 41, 42, 43, 44, 57, 58, 59, 60 |
| Model Select 2 | 73, 74, 75, 76, 89, 90, 91, 92 |

The default MIDI mapping is optimized for [Novation Launch Control XL]: You can
change model with the track buttons and control parameters with the track
faders. The 7th and 8th panning knobs are used to tweak the hue shift values.

[Novation Launch Control XL]: https://novationmusic.com/launch/launch-control-xl

How to train pix2pix models
---------------------------

**Prerequisites**: Basic experience on a machine learning framework. It's
recommended to have a look into the [pix2pix tutorial] from [Machine Learning
for Artists] if you haven't tried pix2pix or any similar GAN model.

You can train your own pix2pix model using [pix2pix-tensorflow]. I personally
recommend using [Google Colaboratory] for training. It's easy to set up, handy
to use and cost effective (it's free!).

The followings are Colaboratory notebooks I used to train the default models.
These notebooks are written to use Google Drive as a dataset storage.

- [Video preprocess]: Converts a video clip into AtoB image pairs.
- [pix2pix-tensorflow colab]
- [pix2pix export]: Exports a `.pict` file from a checkpoint of a trained model.

[pix2pix tutorial]: https://ml4a.github.io/guides/Pix2Pix/
[Machine Learning for Artists]: https://ml4a.github.io/ml4a/
[pix2pix-tensorflow]: https://github.com/affinelayer/pix2pix-tensorflow
[Google Colaboratory]: https://colab.research.google.com
[Video preprocess]: https://colab.research.google.com/drive/1-kmaDxXddLILsByglFD0draX5ha0TRIx
[pix2pix-tensorflow colab]: https://colab.research.google.com/drive/182CGDnFxt08NmjCCTu5jDweUjn3jhB2y
[pix2pix export]: https://colab.research.google.com/drive/1Dc3E6GJ4jjlBIJLaJd--FgKFQGpa_O5N

Frequently asked questions
--------------------------

#### Can't open the project on Unity Editor. ".pict" files are missing.

`.pict` (pix2pix weight data) files are not contained in this repository to
save bandwidth and storage usage. Please download one of the pre-built
executables and copy `.pict` files from the `StreamingAssets` directory
that can be found in the executable package.

#### What are the hardware recommendations?

I used a Windows system with GeForce GTX 1070 for a live performance. It
runs at around 21 fps; this might be the minimum practical spec for the
system.
