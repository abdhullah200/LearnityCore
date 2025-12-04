using LSC.OnlineCourse.Core.Models;
using LSC.RestaurantTableBookingApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LSC.OnlineCourse.API.Controllers
{
    /// <summary>
    /// Provides endpoints for handling contact-related operations, such as sending messages.
    /// </summary>
    /// <remarks>This controller is designed to handle incoming contact messages and process them by sending
    /// email notifications. It is accessible anonymously and does not require authentication.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ContactController : ControllerBase
    {
        /// <summary>
        /// Represents the email notification service used to send email notifications.
        /// </summary>
        /// <remarks>This field is intended to store a reference to an implementation of the <see
        /// cref="IEmailNotification"/> interface. It is used internally to handle email-related operations.</remarks>
        private readonly IEmailNotification emailNotification;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactController"/> class.
        /// </summary>
        /// <remarks>The <see cref="ContactController"/> depends on an implementation of <see
        /// cref="IEmailNotification"/>  to handle email-related functionality. Ensure that a valid implementation is
        /// provided when creating  an instance of this class.</remarks>
        /// <param name="emailNotification">An instance of <see cref="IEmailNotification"/> used to send email notifications.</param>
        public ContactController(IEmailNotification emailNotification)
        {
            this.emailNotification = emailNotification;
        }

        /// <summary>
        /// Sends a contact message via email and returns a success response.
        /// </summary>
        /// <param name="contactMessage">The contact message to be sent. This parameter must not be <see langword="null"/>.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  Returns <see
        /// cref="BadRequestObjectResult"/> if <paramref name="contactMessage"/> is <see langword="null"/>.  Otherwise,
        /// returns <see cref="OkObjectResult"/> with a success message and the sent contact message.</returns>
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ContactMessage contactMessage)
        {
            if (contactMessage == null)
            {
                return BadRequest("Contact message cannot be null.");
            }

            await emailNotification.SendEmailForContactUs(contactMessage);

            return Ok(new { message = "Message sent successfully!" , model= contactMessage });
        }
    }
}
