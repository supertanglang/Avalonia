﻿using Avalonia.Controls;
using Avalonia.Input.Raw;
using Avalonia.Layout;
using Avalonia.Rendering;
using Avalonia.UnitTests;
using Avalonia.VisualTree;
using Moq;
using System;
using Xunit;

namespace Avalonia.Input.UnitTests
{
    public class MouseDeviceTests
    {
        [Fact]
        public void MouseMove_Should_Update_PointerOver()
        {
            var renderer = new Mock<IRenderer>();

            using (TestApplication(renderer.Object))
            {
                var inputManager = InputManager.Instance;
                var mouseDevice = AvaloniaLocator.Current.GetService<IMouseDevice>();

                Canvas canvas;
                Border border;
                Decorator decorator;

                var root = new TestRoot
                {
                    Child = new Panel
                    {
                        Children =
                        {
                            (canvas = new Canvas()),
                            (border = new Border
                            {
                                Child = decorator = new Decorator(),
                            })
                        }
                    }
                };

                renderer.Setup(x => x.HitTest(It.IsAny<Point>(), It.IsAny<Func<IVisual, bool>>()))
                    .Returns(new[] { decorator });

                inputManager.ProcessInput(new RawMouseEventArgs(
                    mouseDevice,
                    0,
                    root,
                    RawMouseEventType.Move,
                    new Point(),
                    InputModifiers.None));

                Assert.True(decorator.IsPointerOver);
                Assert.True(border.IsPointerOver);
                Assert.False(canvas.IsPointerOver);
                Assert.True(root.IsPointerOver);

                renderer.Setup(x => x.HitTest(It.IsAny<Point>(), It.IsAny<Func<IVisual, bool>>()))
                    .Returns(new[] { canvas });

                inputManager.ProcessInput(new RawMouseEventArgs(
                    mouseDevice,
                    0,
                    root,
                    RawMouseEventType.Move,
                    new Point(),
                    InputModifiers.None));

                Assert.False(decorator.IsPointerOver);
                Assert.False(border.IsPointerOver);
                Assert.True(canvas.IsPointerOver);
                Assert.True(root.IsPointerOver);
            }
        }

        private IDisposable TestApplication(IRenderer renderer)
        {
            return UnitTestApplication.Start(
                new TestServices(
                    inputManager: new InputManager(),
                    mouseDevice: () => new MouseDevice(),
                    renderer: (root, loop) => renderer));
        }
    }
}