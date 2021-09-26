using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using TestingThings.Controllers;
using TestingThingsTests.Common;
using Xunit;

namespace TestingThingsTests.Controllers
{
    public class MyException : Exception
    {
        private string _setStackTrace;

        public MyException(string message, string stackTrace) : base(message)
        {
            _setStackTrace = stackTrace ?? throw new ArgumentNullException(nameof(stackTrace));
        }

        public override string StackTrace => _setStackTrace;
    }

    public class ErrorControllerTests
    {
        [Theory, AutoMoqData]
        public void ErrorLocalDevelopment_ValidResponse_ReturnsProblemDetails(
            [Frozen] IWebHostEnvironment webHostEnvironment,
            [Frozen] HttpContext httpContext,
            IFeatureCollection featureCollection,
            ProblemDetailsFactory problemDetailsFactory,
            MyException exception,
            [Greedy] ErrorController sut)
        {
            DecorateMockController(sut, httpContext, problemDetailsFactory);
            SetupWebHostEnvironment(webHostEnvironment, "Development");
            SetupFeatureCollection(httpContext, featureCollection, exception);

            IActionResult problem = sut.ErrorLocalDevelopment(webHostEnvironment);
            ObjectResult objectResult = problem as ObjectResult;
            ProblemDetails returnedProblemDetails = objectResult.Value as ProblemDetails;

            returnedProblemDetails.Should()
                .BeOfType<ProblemDetails>();
        }

        [Theory, AutoMoqData]
        public void ErrorLocalDevelopment_ValidResponse_ReturnsProblemDetailsWithErrorStackAsDetails(
            [Frozen] IWebHostEnvironment webHostEnvironment,
            [Frozen] HttpContext httpContext,
            IFeatureCollection featureCollection,
            ProblemDetailsFactory problemDetailsFactory,
            MyException exception,
            [Greedy] ErrorController sut)
        {
            DecorateMockController(sut, httpContext, problemDetailsFactory);
            SetupWebHostEnvironment(webHostEnvironment, "Development");
            SetupFeatureCollection(httpContext, featureCollection, exception);

            IActionResult problem = sut.ErrorLocalDevelopment(webHostEnvironment);
            ObjectResult objectResult = problem as ObjectResult;
            ProblemDetails returnedProblemDetails = objectResult.Value as ProblemDetails;

            returnedProblemDetails.Detail.Should()
                .Be(exception.StackTrace);
        }

        [Theory, AutoMoqData]
        public void Error_ValidResponse_ReturnsProblemDetails(
            [Greedy] ErrorController sut)
        {
            IActionResult problem = sut.Error();
            ObjectResult objectResult = problem as ObjectResult;
            ProblemDetails returnedProblemDetails = objectResult.Value as ProblemDetails;

            returnedProblemDetails.Should()
                .BeOfType<ProblemDetails>();
        }

        [Theory, AutoMoqData]
        public void ErrorLocal_ValidResponse_DoesNotReturnProblemDetailsWithErrorStackAsDetails(
            [Greedy] ErrorController sut)
        {
            IActionResult problem = sut.Error();
            ObjectResult objectResult = problem as ObjectResult;
            ProblemDetails returnedProblemDetails = objectResult.Value as ProblemDetails;

            returnedProblemDetails.Detail.Should()
                .BeNullOrEmpty();
        }

        private static void SetupFeatureCollection(HttpContext httpContext, IFeatureCollection featureCollection, Exception exception)
        {
            featureCollection.AsMock()
                .Setup(fc => fc.Get<IExceptionHandlerFeature>())
                .Returns(new ExceptionHandlerFeature() {
                    Error = exception
                });

            httpContext.AsMock()
                .Setup(hc => hc.Features)
                .Returns(featureCollection);
        }

        private static void SetupWebHostEnvironment(IWebHostEnvironment webHostEnvironment, string environmentName)
        {
            webHostEnvironment.AsMock()
                .SetupGet(w => w.EnvironmentName)
                .Returns(environmentName);
        }

        private static void DecorateMockController(ControllerBase controllerBase, HttpContext httpContext, ProblemDetailsFactory problemDetailsFactory)
        {
            controllerBase.ControllerContext = new();
            controllerBase.ControllerContext.HttpContext = httpContext;
            controllerBase.ProblemDetailsFactory = problemDetailsFactory;
        }
    }
}
