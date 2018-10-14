# Ngx - Neural network based visual generator and mixer

**Ngx** is an attempt at utilizing a neural network for VJing. It uses
[pix2pix] (image-to-image translation with cGAN) as an ad-hoc next-frame
prediction model -- it's trained with pairs of consecutive frames extracted
from a video clip, so that it can generate successive frames starting from an
arbitrary image for an infinite duration just by feeding frames back.

[pix2pix]: https://phillipi.github.io/pix2pix/

![gif](https://i.imgur.com/2WMKfdA.gif)
![gif](https://i.imgur.com/sMgvbo6.gif)

(The gif on the right is from a tweet by
[@chaosgroove](https://twitter.com/chaosgroove/status/1051082769843990528))

[Vimeo - Ngx demonstration](https://vimeo.com/294399440)

Pre-built executable
--------------------

You can download a pre-built executable from [Releases] page. It contains some
pre-trained models with [Beeple]'s VJ clips.

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

The original video clips are released under a Creative Commons Attribution
license (CC-BY). Also these pre-trained models should be attributed to the
original author.

[Releases]: https://github.com/keijiro/Ngx/releases
[Beeple]: https://www.beeple-crap.com/

Controller
----------

Ngx can be controlled with the on-screen controller or a MIDI controller.

![screenshot](https://i.imgur.com/CEOu0I1m.jpg)

| Name      | Function                           | MIDI CC |
| --------- | ---------------------------------- | ------- |
| Mix       | Mixing between model 1 and model 2 | 77      |
| Noise 1   | Noise injection (low density)      | 78      |
| Noise 2   | Noise injection (medium density)   | 79      |
| Noise 3   | Noise injection (high density)     | 80      |

| Name      | Function                           | MIDI CC |
| --------- | ---------------------------------- | ------- |
| Feedback  | Feedback rate                      | 81, 82  |
| FColor    | False color effect                 | 83      |
| Hue Shift | Amount of hue shift                | 55, 56  |
| Invert    | Color inversion                    | 84      |

