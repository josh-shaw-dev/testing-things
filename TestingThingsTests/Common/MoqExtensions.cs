﻿using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Text.RegularExpressions;

namespace TestingThingsTests.Common
{
    public static class MoqExtensions
    {
        public static Mock<T> AsMock<T>(this T implementation) where T : class => Mock.Get(implementation);

        public static void VerifyLogWithLogLevel<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, Times times)
        {
            Func<object, Type, bool> state = (v, t) => true;
            Verify(logger, logLevel, times, state);
        }

        public static void VerifyLogWithLogLevel<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, Func<Times> times)
        {
            VerifyLogWithLogLevel(logger, logLevel, times());
        }

        public static void VerifyLogWithLogLevelAndContainsMessage<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, Times times, string containsMessage)
        {
            Func<object, Type, bool> state = (v, t) => v.ToString().Contains(containsMessage);
            Verify(logger, logLevel, times, state);
        }

        public static void VerifyLogWithLogLevelAndContainsMessage<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, Times times, Regex containsRegex)
        {
            Func<object, Type, bool> state = (v, t) => containsRegex.IsMatch(v.ToString());
            Verify(logger, logLevel, times, state);
        }

        public static void VerifyLogWithLogLevelAndContainsMessage<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, Func<Times> times, string containsMessage)
        {
            VerifyLogWithLogLevelAndContainsMessage(logger, logLevel, times(), containsMessage);
        }

        public static void VerifyLogWithLogLevelAndContainsMessage<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, Func<Times> times, Regex containsRegex)
        {
            VerifyLogWithLogLevelAndContainsMessage(logger, logLevel, times(), containsRegex);
        }

        private static void Verify<T>(Mock<ILogger<T>> logger, LogLevel logLevel, Times times, Func<object, Type, bool> messageCheck)
        {
            logger
                .Verify(
                    l => l.Log(
                        It.Is<LogLevel>(ll => ll == logLevel),
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => messageCheck(v, t)),
                        It.IsAny<Exception>(),
                        It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                    times,
                    "Failed to log amount/message as expected"
                );
        }
    }
}
