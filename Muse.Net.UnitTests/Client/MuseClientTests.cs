using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Muse.Net.Client;
using Muse.Net.Models.Enums;
using Muse.Net.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Muse.Net.UnitTests.Client
{
    [TestClass]
    public class MuseClientTests
    {
        [TestMethod]
        public async Task GivenMuseClient_WhenConnect_ThenBluetoothClientConnectCalled_AndChannelsConfigured()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var deviceId = (ulong)100;
            var sut = new MuseClient(mockBluetoothClient.Object);

            mockBluetoothClient.Setup(x => x.Connect(
                It.IsAny<ulong>(),
                It.IsAny<System.Guid>(),
                It.IsAny<KeyValuePair<Channel, System.Guid>[]>()))
                .ReturnsAsync(true);

            // Act
            var result = await sut.Connect(deviceId);

            // Assert
            Assert.IsTrue(result);
            mockBluetoothClient.Verify(x => x.Connect(
                It.Is<ulong>(y => y == deviceId),
                It.Is<System.Guid>(y => y == MuseGuid.PRIMARY_SERVICE),
                It.Is<KeyValuePair<Channel, System.Guid>[]>(y => AssertConnectionChannels(y))), Times.Once);
        }

        [TestMethod]
        public async Task GivenMuseClient_WhenDisconnect_ThenBluetoothClientDisconnected()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var sut = new MuseClient(mockBluetoothClient.Object);

            mockBluetoothClient.Setup(x => x.UnsubscribeAll())
                .Returns(Task.CompletedTask);

            mockBluetoothClient.Setup(x => x.Disconnect())
                .Returns(Task.CompletedTask);

            // Act
            await sut.Disconnect();

            // Assert
            mockBluetoothClient.Verify(x => x.UnsubscribeAll(), Times.Once);
            mockBluetoothClient.Verify(x => x.Disconnect(), Times.Once);
        }

        [TestMethod]
        public async Task GivenChannels_WhenSubscribe_ThenBluetoothClientSubscribeToChannelCalled()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var channels = new Channel[]
            {
                Channel.Accelerometer,
                Channel.Control,
                Channel.EEG_AF7,
                Channel.EEG_AF8,
                Channel.EEG_AUX,
                Channel.EEG_TP10,
                Channel.EEG_TP9,
                Channel.Gyroscope,
                Channel.Telemetry
            };
            var sut = new MuseClient(mockBluetoothClient.Object);

            mockBluetoothClient.Setup(x => x.SubscribeToChannel(
                It.IsAny<Channel>()))
                .ReturnsAsync(true);

            // Act
            await sut.Subscribe(channels);

            // Assert
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.Accelerometer)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.Control)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.EEG_AF7)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.EEG_AF8)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.EEG_AUX)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.EEG_TP10)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.EEG_TP9)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.Gyroscope)), Times.Once);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.Is<Channel>(y => y == Channel.Telemetry)), Times.Once);
        }

        [TestMethod]
        public async Task GivenMuseClient_WhenUnsubscribeAll_ThenBluetoothClientUnsubscribeAll()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var sut = new MuseClient(mockBluetoothClient.Object);

            mockBluetoothClient.Setup(x => x.UnsubscribeAll())
                .Returns(Task.CompletedTask);

            // Act
            await sut.UnsubscribeAll();

            // Assert
            mockBluetoothClient.Verify(x => x.UnsubscribeAll(), Times.Once);
        }

        [TestMethod]
        public async Task GivenMuseClient_WhenResume_ThenCharacteristicControlWriteCommand()
        {
            // Arrange
            var mockGattCharacteristic = new Mock<IGattCharacteristic>();
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var sut = new MuseClient(mockBluetoothClient.Object);

            mockBluetoothClient.SetupGet(x => x.Characteristics[Channel.Control])
                .Returns(mockGattCharacteristic.Object);

            // Act
            await sut.Resume();

            // Assert
            mockGattCharacteristic.Verify(x => x.WriteCommand(
                It.Is<string>(y => y == MuseCommand.RESUME)), Times.Once);
        }

        [TestMethod]
        public async Task GivenMuseClient_WhenPause_ThenCharacteristicControlWriteCommand()
        {
            // Arrange
            var mockGattCharacteristic = new Mock<IGattCharacteristic>();
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var sut = new MuseClient(mockBluetoothClient.Object);

            mockBluetoothClient.SetupGet(x => x.Characteristics[Channel.Control])
                .Returns(mockGattCharacteristic.Object);

            // Act
            await sut.Pause();

            // Assert
            mockGattCharacteristic.Verify(x => x.WriteCommand(
                It.Is<string>(y => y == MuseCommand.PAUSE)), Times.Once);
        }

        [TestMethod]
        public async Task GivenMuseClient_WhenStart_ThenCharacteristicControlWriteCommand()
        {
            // Arrange
            var mockGattCharacteristic = new Mock<IGattCharacteristic>();
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var sut = new MuseClient(mockBluetoothClient.Object);

            mockBluetoothClient.SetupGet(x => x.Characteristics[Channel.Control])
                .Returns(mockGattCharacteristic.Object);

            // Act
            await sut.Start();

            // Assert
            mockGattCharacteristic.Verify(x => x.WriteCommand(
                It.Is<string>(y => y == MuseCommand.START)), Times.Once);
        }

        [TestMethod]
        public async Task GivenChannel_WhenSubscribeToChannel_ThenBluetoothClientSubscribeToChannel_AndResultReturned()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var channel = Channel.EEG_AF7;
            var sut = new MuseClient(mockBluetoothClient.Object);

            mockBluetoothClient.Setup(x => x.SubscribeToChannel(
                It.IsAny<Channel>()))
                .ReturnsAsync(true);

            // Act
            var result = await sut.SubscribeToChannel(channel);

            // Assert
            Assert.IsTrue(result);
            mockBluetoothClient.Verify(x => x.SubscribeToChannel(
                It.IsAny<Channel>()), Times.Once);
        }

        [TestMethod]
        public async Task GivenChannel_WhenUnsubscribeFromChannel_ThenBluetoothClientUnsubscribeFromChannel_AndResultReturned()
        {
            // Arrange
            var mockBluetoothClient = new Mock<IBluetoothClient<Channel, IGattCharacteristic>>();
            var channel = Channel.EEG_AF7;
            var sut = new MuseClient(mockBluetoothClient.Object);

            mockBluetoothClient.Setup(x => x.UnsubscribeFromChannel(
                It.IsAny<Channel>()))
                .ReturnsAsync(true);

            // Act
            var result = await sut.UnsubscribeFromChannel(channel);

            // Assert
            Assert.IsTrue(result);
            mockBluetoothClient.Verify(x => x.UnsubscribeFromChannel(
                It.IsAny<Channel>()), Times.Once);
        }

        private bool AssertConnectionChannels(KeyValuePair<Channel, System.Guid>[] connectionChannels)
        {
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.Accelerometer));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.Control));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.EEG_AF7));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.EEG_AF8));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.EEG_AUX));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.EEG_TP10));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.EEG_TP9));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.Gyroscope));
            Assert.IsTrue(connectionChannels.Any(x => x.Key == Channel.Telemetry));
            return true;
        }
    }
}
