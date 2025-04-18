namespace Application.Contracts;

public interface ICommandHandler<TCommand, TCommandResult>
    where TCommand : ICommand
{
    Task<TCommandResult> Handle(TCommand command);
}

public interface ICommand { }
