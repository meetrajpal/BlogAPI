namespace BlogAPI.API.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/comments")]
[ApiVersion("1.0")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("post/{postId:guid}")]
    public async Task<IActionResult> GetByPost(Guid postId, [FromQuery] PaginationFilter filter)
    {
        var result = await _commentService.GetCommentsByPostAsync(postId, filter);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _commentService.GetCommentByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
    {
        var authorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _commentService.CreateCommentAsync(dto, authorId);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var authorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isAdmin = User.IsInRole("Admin");
        var result = await _commentService.DeleteCommentAsync(id, authorId, isAdmin);
        return Ok(result);
    }
}