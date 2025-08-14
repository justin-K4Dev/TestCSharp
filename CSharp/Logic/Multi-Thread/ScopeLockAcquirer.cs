using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;




namespace MultiThread
{
    public class ScopeLockAcquirer : IDisposable
    {
        private readonly object[] _lockObjects;
        private bool _locksAcquired;

        // 지정된 락 객체들을 제공된 순서대로 획득을 시도합니다.
        // 이 순서를 모든 스레드가 일관되게 지키는 것이 Deadlock 방지의 핵심입니다.
        // locks : 획득할 락 객체들의 목록입니다. 이 리스트의 순서가 락 획득 순서가 됩니다
        public ScopeLockAcquirer(IEnumerable<object> locks) // 생성자 이름도 클래스 이름에 맞춰 변경
        {
            if (locks == null) throw new ArgumentNullException(nameof(locks));
            _lockObjects = locks.ToArray();

            _locksAcquired = true;

            foreach (var obj in _lockObjects)
            {
                System.Threading.Monitor.Enter(obj);
                Console.WriteLine($"[Thread {System.Threading.Thread.CurrentThread.ManagedThreadId}] Acquired lock on object: {obj.GetHashCode()}");
            }
        }

        // 획득한 모든 락을 해제합니다. using 문을 벗어날 때 자동으로 호출됩니다.
        public void Dispose()
        {
            if (_locksAcquired)
            {
                // 획득한 락을 역순으로 해제하는 것이 일반적인 관례입니다.
                for (int i = _lockObjects.Length - 1; i >= 0; i--)
                {
                    System.Threading.Monitor.Exit(_lockObjects[i]);
                    Console.WriteLine($"[Thread {System.Threading.Thread.CurrentThread.ManagedThreadId}] Released lock on object: {_lockObjects[i].GetHashCode()}");
                }
                _locksAcquired = false;
            }
        }
    }

    public class Account
    {
        public Guid Id { get; private set; }
        public decimal Balance { get; private set; }
        private readonly object _lock = new object();

        public Account(decimal initialBalance)
        {
            Id = Guid.NewGuid();
            Balance = initialBalance;
        }

        public Account(string id, decimal initialBalance)
        {
            Id = new Guid(id);
            Balance = initialBalance;
        }

        public object GetLockObject()
        {
            return _lock;
        }

        public void Deposit(decimal amount)
        {
            lock (_lock)
            {
                Balance += amount;
                Console.WriteLine($"Account {Id.ToString().Substring(0, 8)}: Deposited {amount}. New Balance: {Balance}");
            }
        }

        public bool Withdraw(decimal amount)
        {
            lock (_lock)
            {
                if (Balance >= amount)
                {
                    Balance -= amount;
                    Console.WriteLine($"Account {Id.ToString().Substring(0, 8)}: Withdrew {amount}. New Balance: {Balance}");
                    return true;
                }
                Console.WriteLine($"Account {Id.ToString().Substring(0, 8)}: Insufficient funds to withdraw {amount}. Current Balance: {Balance}");
                return false;
            }
        }
    }

    public class DeadLockAvoidExample
    {
        public void transfer( Account fromAccount, Account toAccount
                            , decimal amount )
        {
            Console.WriteLine($"Attempting to transfer {amount} from Account {fromAccount.Id.ToString().Substring(0, 8)} to Account {toAccount.Id.ToString().Substring(0, 8)}");

            List<Account> accountsToLock = new List<Account>
            {
                fromAccount,
                toAccount
            };

            // 핵심 변경: AccountWithGuidId 객체들을 ID(Guid)를 기준으로 오름차순 정렬합니다.
            // 이렇게 정렬된 순서로 락을 획득하면 Deadlock을 피할 수 있습니다.
            var sortedAccounts = accountsToLock.OrderBy(account => account.Id).ToList();

            // 정렬된 Account 객체들에서 락 객체만 추출하여 ScopeLockAcquirer에 전달합니다.
            List<object> orderedLocks = sortedAccounts.Select(acc => acc.GetLockObject()).ToList();

            try
            {
                // ScopeLockAcquirer를 사용하여 정의된 순서대로 락을 획득합니다.
                using (new ScopeLockAcquirer(orderedLocks)) // 클래스 이름 변경 적용
                {
                    Console.WriteLine($"[Thread {System.Threading.Thread.CurrentThread.ManagedThreadId}] All locks acquired for transfer from Account {fromAccount.Id.ToString().Substring(0, 8)} to Account {toAccount.Id.ToString().Substring(0, 8)}");
                    System.Threading.Thread.Sleep(50); // 작업 시뮬레이션

                    if (fromAccount.Withdraw(amount))
                    {
                        toAccount.Deposit(amount);
                        Console.WriteLine($"Successfully transferred {amount} from Account {fromAccount.Id.ToString().Substring(0, 8)} to Account {toAccount.Id.ToString().Substring(0, 8)}");
                    }
                    else
                    {
                        Console.WriteLine($"Transfer failed due to insufficient funds in Account {fromAccount.Id.ToString().Substring(0, 8)}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Thread {System.Threading.Thread.CurrentThread.ManagedThreadId}] Transfer failed due to lock acquisition error !!! : exception:{e}");
            }
        }

        public static void Test()
        {
            var acc1 = new Account(1000);
            var acc2 = new Account(500);

            var dead_lock_avoid_example = new DeadLockAvoidExample();

            Console.WriteLine("--- Starting Guid Lock Ordering Example (Sorted List) ---");
            Console.WriteLine($"Account 1 ID: {acc1.Id}");
            Console.WriteLine($"Account 2 ID: {acc2.Id}");

            // 스레드 1: acc1 -> acc2로 이체
            Task t1 = Task.Run(() => dead_lock_avoid_example.transfer(acc1, acc2, 200));

            // 스레드 2: acc2 -> acc1로 이체
            Task t2 = Task.Run(() => dead_lock_avoid_example.transfer(acc2, acc1, 100));

            Task.WaitAll(t1, t2);

            Console.WriteLine("\n--- Transfer operations completed ---");
            Console.WriteLine($"Final Balance Account 1: {acc1.Balance}");
            Console.WriteLine($"Final Balance Account 2: {acc2.Balance}");
        }
    }
}
