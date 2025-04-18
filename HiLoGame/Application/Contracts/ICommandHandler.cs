namespace Application.Contracts;

public interface ICommandHandler<TCommand, TCommandResult>
    where TCommand : ICommand
    where TCommandResult : ICommandResult
{
    Task<TCommandResult> Handle(TCommand command);
}

public interface ICommand { }

public interface ICommandResult { }
